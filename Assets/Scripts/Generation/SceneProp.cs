using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class SceneProp : PoolableBehaviour
{
    [Header("Prop Settings")]
    [SerializeField] private BoxCollider bounds;
    [SerializeField] private bool randomizeRotation;
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;

    [Header("Debugging")]
    [SerializeField] private bool debugGround;

    public BoxCollider Bounds => bounds;
    public ChunkInstance Chunk { get; set; }
    private float3 baseScale;

    protected void Awake()
    {
        baseScale = transform.localScale;
    }

    public virtual void OnPlace() { }

    public void ApplyRandomization()
    {
        if (randomizeRotation)
        {
            transform.localEulerAngles = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);
        }

        var scale = UnityEngine.Random.Range(minSize, maxSize);
        transform.localScale = baseScale * scale;
    }

    void Update()
    {
        if (debugGround)
        {
            var position = transform.position.tofloat3().xz;
            GenerationUtils.GroundHeightAt(position, debug: true);
        }
    }
}
