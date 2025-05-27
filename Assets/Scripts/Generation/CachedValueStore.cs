using System.Collections.Generic;
using UnityEngine.Pool;

public class CachedValueStore<ID, K, V>
{
    /// <summary>
    /// A helper struct to store the information needed
    /// by a single gradient in the cache.
    /// </summary>
    private struct Value
    {
        public int useCount;
        public V value;
    }

    /// <summary>
    /// The delegate type responsible for generating new values
    /// when one is not found.
    /// </summary>
    public delegate V Generator(K key);

    private readonly Dictionary<ID, HashSet<K>> usages = new();
    private readonly Dictionary<K, Value> values = new();
    private readonly Generator generator;

    public CachedValueStore(Generator generator)
    {
        this.generator = generator;
    }

    /// <summary>
    /// A helper method to get (or initialize and cache) the set of 
    /// gradients (by position) that are 'used' by the provided chunk id,
    /// <paramref name="chunk"/>.
    /// </summary>
    private HashSet<K> GetUsedValues(ID id)
    {
        if (!usages.TryGetValue(id, out var used))
        {
            used = HashSetPool<K>.Get();
            usages.Add(id, used);
        }

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
            var value = values[grad];
            value.useCount--;

            // And remove it if it is now unused //
            if (value.useCount == 0)
            {
                values.Remove(grad);
            }
            else
            {
                values[grad] = value;
            }
        }

        // Then free the association set //
        usages.Remove(id);
        HashSetPool<K>.Release(used);
    }

    /// <summary>
    /// Get or generate the value associated with the provided identifier and key.
    /// </summary>
    public V Get(ID id, K key)
    {
        var used = GetUsedValues(id);

        if (values.TryGetValue(key, out var value))
        {
            // If the gradient exists, ensure the provided chunk is //
            // marked as using it                                   //
            if (!used.Contains(key))
            {
                used.Add(key);
                value.useCount++;
            }
        }
        else
        {
            // Otherwise, generate a new gradient //
            value = new Value
            {
                useCount = 1,
                value = generator(key)
            };

            used.Add(key);
        }

        values[key] = value;
        return value.value;
    }
}
