using System;
using Unity.Mathematics;
using UnityEngine;

public abstract class ScenePropPlacer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SceneProp prefab;

    // NOTE: this value of 256 is an arbitrary upper bound //
    private readonly Collider[] colliderResults = new Collider[256];


    private InstancePool<SceneProp> instancePool;

    protected void Awake()
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

        if (checkForOverlap)
        {
            if (GenerationUtils.IsPathAt(position))
            {
                permit = false;
            }
            else
            {
                var bounds = prop.Bounds.bounds;

                var count = Physics.OverlapBoxNonAlloc(
                    bounds.center,
                    bounds.extents / 2,
                    colliderResults,
                    prop.transform.rotation,
                    ~Layers.GroundMask & ~Layers.IgnoreRaycastMask);

                for (int i = 0; i < count; i++)
                {
                    if (!colliderResults[i].IsChildOf(prop.transform))
                    {
                        permit = false;
                        break;
                    }
                }
            }
        }

        if (!permit)
        {
            instancePool.Release(prop);
            return null;
        }

        chunk.AttachProp(prop);
        prop.Chunk = chunk;
        prop.OnPlace();
        return prop;
    }
}
