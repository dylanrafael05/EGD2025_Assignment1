using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkInstance : PoolableBehaviour
{
    [SerializeField] private GameObject propContainer;
    [SerializeField] private MeshFilter roadFilter;
    [SerializeField] private MeshCollider roadCollider;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;
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
    /// 
    /// <para>
    /// ! Safety !
    /// <br/>
    /// It is assumed that triangle information, uv information, and vertex x and z coordinates
    /// do not change after this instance is first created.
    /// </para>
    /// </summary>
    public ChunkMesher GroundMesher { get; private set; }
    /// <summary>
    /// The mesh data associated with the 'roads' of this chunk.
    /// 
    /// <para>
    /// ! Safety !
    /// <br/>
    /// It is assumed that triangle information, uv information, and vertex x and z coordinates
    /// do not change after this instance is first created.
    /// </para>
    /// </summary>
    public ChunkMesher PathMesher { get; private set; }

    /// <summary>
    /// Whether or not this chunk is fresh and needs setup, or can rely on already existing data.
    /// </summary>
    public bool NeedsInitialization { get; set; } = true;

    void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();

        GroundMesher ??= new(GeneratorManager.Instance.gridCount, GeneratorManager.Instance.UnitSideLength);
        PathMesher ??= new(GeneratorManager.Instance.gridCount, GeneratorManager.Instance.UnitSideLength);

        /*//
            DebugLabels.Attach(
                gameObject,
                Vector3.up * 5,
                () => GeneratorManager.Instance.CalcForestDampen(ID, transform.position.tofloat3().xz)
            );
        //*/
    }

    public void UpdateMeshInfo()
    {
        GroundMesher.UpdateMeshInfo();
        PathMesher.UpdateMeshInfo();

        _filter.mesh = GroundMesher.Mesh;
        _collider.sharedMesh = GroundMesher.Mesh;

        roadFilter.mesh = PathMesher.Mesh;
        roadCollider.sharedMesh = PathMesher.Mesh;
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
    }
}
