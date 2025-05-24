using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;

    public ChunkID ID { get; set; }
    public float2 Position { get; set; }

    void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();
    }

    public void SetMesh(Mesh mesh)
    {
        _filter.mesh = mesh;
        _collider.sharedMesh = mesh;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
