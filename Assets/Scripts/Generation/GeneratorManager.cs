using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    private MeshFilter meshFilter;
    private PersistentNoise noise;
    
    [Header("Settings")]
    public int gridCount;
    public float unitSideLength;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        noise = new();

        var gen = new ChunkMeshBuilder(gridCount, unitSideLength);
        meshFilter.mesh = gen.Build();
    }
}
