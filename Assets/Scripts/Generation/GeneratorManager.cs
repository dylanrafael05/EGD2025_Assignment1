using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    private MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh = Generator.BuildSimpleGridMesh();
    }
}
