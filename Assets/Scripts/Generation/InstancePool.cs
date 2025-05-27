using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInstancePool
{
    object Get();
    void Release(object obj);
}

public static class InstancePool
{
    private static readonly Dictionary<object, IInstancePool> pooledInstanceMap = new();

    public static void RegisterObject(object obj, IInstancePool pool)
    {
        pooledInstanceMap.Add(obj, pool);
    }

    public static IInstancePool GetAssociatedPool(object obj)
    {
        if (!pooledInstanceMap.TryGetValue(obj, out var pool))
            return null;

        return pool;
    }

    public static InstancePool<T> OfPrefab<T>(T prefab, Transform parent = null)
        where T : PoolableBehaviour
        => new(
            create: () => GameObject.Instantiate(prefab, parent),
            onGet: x => x.Activate(),
            onRelease: x => x.Deactivate()
        );

    public static void Release(object obj)
    {
        var pool = GetAssociatedPool(obj)
            ?? throw new ArgumentNullException($"Provided object ({obj}) was not allocated in from a pool.");

        pool.Release(obj);
    }
}

public class InstancePool<T> : IInstancePool
    where T : class
{
    private readonly Func<T> create;
    private readonly Action<T> onGet;
    private readonly Action<T> onRelease;

    private readonly List<T> freeInstances = new();

    public InstancePool(Func<T> create, Action<T> onGet, Action<T> onRelease)
    {
        this.create = create;
        this.onGet = onGet;
        this.onRelease = onRelease;
    }

    private T CreateAndRegister()
    {
        var instance = create();

        InstancePool.RegisterObject(instance, this);
        return instance;
    }

    private T GetWithoutCallback()
    {
        if (freeInstances.Count != 0)
        {
            var instance = freeInstances[^1];
            freeInstances.RemoveAt(freeInstances.Count - 1);

            return instance;
        }

        return CreateAndRegister();
    }

    public T Get()
    {
        var instance = GetWithoutCallback();
        onGet(instance);

        return instance;
    }

    object IInstancePool.Get()
        => Get();

    public void Release(T instance)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        var pool = InstancePool.GetAssociatedPool(instance);

        if (pool != this)
            throw new ArgumentException("Instance was not allocated in this pool.", nameof(instance));

        onRelease(instance);
        freeInstances.Add(instance);
    }

    void IInstancePool.Release(object obj)
        => Release(obj as T);
}