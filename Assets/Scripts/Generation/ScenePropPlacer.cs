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

    protected SceneProp AttemptCreate(ChunkInstance chunk, float2 position, float heightFromGround = 0, bool checkForOverlap = false)
    {
        var prop = instancePool.Get();

        var groundedPosition = position.xxy;
        groundedPosition.y = heightFromGround + GenerationUtils.GroundHeightAt(position);

        prop.transform.position = groundedPosition;
        var permit = true;

        if (checkForOverlap)
        {
            var bounds = prop.Bounds.bounds;

            var count = Physics.OverlapBoxNonAlloc(
                bounds.center,
                bounds.extents / 2,
                colliderResults,
                prop.transform.rotation,
                ~Layers.GroundMask);

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
