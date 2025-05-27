using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkInstance : PoolableBehaviour
{
    [SerializeField] private GameObject propContainer;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;
    private ChunkMesher mesh;
    private List<PoolableBehaviour> props = new();

    /// <summary>
    /// The unique ID of this chunk.
    /// </summary>
    public ChunkID ID { get; set; }
    /// <summary>
    /// The position of this chunk in chunk coordinates.
    /// </summary>
    public int2 Position { get; set; }
    /// <summary>
    /// The bounding box of the chunk in the xz-plane in world space.
    /// </summary>
    public Rect Bounds { get; set; }
    /// <summary>
    /// The mesh data associated with this chunk.
    /// </summary>
    public ChunkMesher Mesher => mesh;

    void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();

        _filter.mesh = Mesher.Mesh;
        _collider.sharedMesh = Mesher.Mesh;

        print("Erm what the fuck.");

        mesh = new(GeneratorManager.Instance.gridCount, GeneratorManager.Instance.UnitSideLength);
    }

    public void AttachProp(PoolableBehaviour prop)
    {
        props.Add(prop);
        prop.transform.parent = propContainer.transform;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        foreach (var prop in props)
        {
            InstancePool.Release(prop);
        }

        props.Clear();
        Mesher.Reset();
    }
}
