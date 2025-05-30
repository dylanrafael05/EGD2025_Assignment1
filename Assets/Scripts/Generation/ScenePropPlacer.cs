using System;
using Unity.Mathematics;
using UnityEngine;

public abstract class ScenePropPlacer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SceneProp prefab;

    private readonly Collider[] colliderResults = new Collider[2];


    private InstancePool<SceneProp> instancePool;

    void Awake()
    {
        instancePool = InstancePool.OfPrefab(prefab);
    }

    public abstract void PlaceInChunk(ChunkInstance chunk);

    protected SceneProp AttemptCreate(ChunkInstance chunk, float2 position, bool checkForOverlap = true)
    {
        if (!chunk.Bounds.Contains(position))
            throw new InvalidOperationException("Cannot place an object in a chunk if it is not actually in that object.");

        var prop = instancePool.Get();

        var pos = position.xxy;
        pos.y = GenerationUtils.GroundHeightAt(position);

        prop.transform.position = pos;
        prop.ApplyRandomization();

        var permit = true;

        if (GenerationUtils.IsPathAt(position))
        {
            permit = false;
        }
        else if (checkForOverlap)
        {
            var bounds = prop.Bounds.bounds;

            var count = Physics.OverlapBoxNonAlloc(
                bounds.center,
                bounds.extents / 2,
                colliderResults,
                prop.transform.rotation,
                ~Layers.GroundMask & ~Layers.IgnoreRaycastMask);

            permit = (count <= 1);
        }

        if (!permit)
        {
            instancePool.Release(prop);

            return null;
        }

        chunk.AttachProp(prop);
        return prop;
    }
}
