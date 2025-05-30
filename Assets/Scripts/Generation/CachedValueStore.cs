using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

public class CachedValueStore<ID, K, V>
{
    /// <summary>
    /// A helper struct to store the information needed
    /// by a single gradient in the cache.
    /// </summary>
    private class ValueStorage
    {
        public int useCount = -1;
        public V value;
    }

    /// <summary>
    /// The delegate type responsible for generating new values
    /// when one is not found.
    /// </summary>
    public delegate V Generator(K key);

    private readonly ConcurrentDictionary<ID, ConcurrentSet<K>> usages = new();
    private readonly ConcurrentDictionary<K, ValueStorage> values = new();
    private readonly Generator generator;
    private readonly Mutex generateMut = new();
    private readonly Mutex usagesMut = new();
    private readonly InstancePool<ConcurrentSet<K>> usagesPool;
    private readonly InstancePool<ValueStorage> valuesPool;

    public CachedValueStore(Generator generator)
    {
        this.generator = generator;

        usagesPool = new(
            create: () => new(),
            onGet: s => s.Clear()
        );

        valuesPool = new(
            create: () => new()
        );
    }

    /// <summary>
    /// A helper method to get (or initialize and cache) the set of 
    /// gradients (by position) that are 'used' by the provided chunk id,
    /// <paramref name="chunk"/>.
    /// </summary>
    private ConcurrentSet<K> GetUsedValues(ID id)
    {
        ConcurrentSet<K> used;

        if (usages.TryGetValue(id, out used))
            return used;

        usagesMut.WaitOne();

        if (usages.TryGetValue(id, out used))
        {
            usagesMut.ReleaseMutex();
            return used;
        }

        used = usagesPool.Get();
        usages.TryAdd(id, used);

        usagesMut.ReleaseMutex();
        return used;
    }

    /// <summary>
    /// Unload all values associated with the provided id.
    /// </summary>
    public void UnloadID(ID id)
    {
        // For every gradient used by this chunk id //
        var used = GetUsedValues(id);

        foreach (var grad in used)
        {
            // Remove one use from its counter //
            values.TryGetValue(grad, out var value);
            Interlocked.Decrement(ref value.useCount);

            // And remove it if it is now unused //
            if (value.useCount == 0)
            {
                values.TryRemove(grad, out _);
                valuesPool.Release(value);
            }
        }

        // Then free the association set //
        usages.TryRemove(id, out _);
        usagesPool.Release(used);
    }

    /// <summary>
    /// Get the value (and update the id set) from a pre-existing value.
    /// </summary>
    private V GetFromExisting(ID id, K key, ValueStorage value)
    {
        var used = GetUsedValues(id);

        // If the gradient exists, ensure the provided chunk is //
        // marked as using it                                   //
        if (!used.Contains(key))
        {
            Interlocked.Increment(ref value.useCount);
            used.Add(key);
        }

        return value.value;
    }

    /// <summary>
    /// Get or generate the value associated with the provided identifier and key.
    /// </summary>
    public V Get(ID id, K key)
    {
        // If the value storage already exists, use it directly //
        ValueStorage value;

        if (values.TryGetValue(key, out value))
            return GetFromExisting(id, key, value);

        // Otherwise, wait for the mutex to try and create it //
        if (!generateMut.WaitOne())
        {
            Debug.LogError($"Mutex acquistion failed! This is a major issue.");
        }

        // If another thread has already created it (i.e. it got here first),
        // then use the value it calculated.
        if (values.TryGetValue(key, out value))
        {
            generateMut.ReleaseMutex();
            return GetFromExisting(id, key, value);
        }

        // Otherwise, generate a new storage instance //
        var used = GetUsedValues(id);

        value = valuesPool.Get();
        value.useCount = 1;
        value.value = generator(key);

        used.Add(key);
        values.TryAdd(key, value);

        // Release the mutex //
        generateMut.ReleaseMutex();
        return value.value;
    }
}
