using Unity.Mathematics;
using UnityEngine;

public readonly struct ChunkMeshBuilder
{
    // Private fields //
    private readonly int gridCount;
    private readonly float unitSideLength;
    private readonly Vector3[] vertices;
    private readonly Color[] colors;
    private readonly Vector2[] uvs;
    private readonly int[] triangles;

    /// <summary>
    /// An adaptor around some array which provides multidimensional
    /// indexing, for the purposes of simple iteration over vertex information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Adaptor<T>
    {
        private readonly T[] array;
        private readonly int gridCount;

        public Adaptor(T[] array, int gridCount)
        {
            this.array = array;
            this.gridCount = gridCount;
        }

        public ref T this[int2 xy]
            => ref this[xy.x, xy.y];
        public ref T this[int x, int y]
            => ref array[x * gridCount + y];
    }

    // Public APIs //
    public int GridCount => gridCount;
    public float UnitSideLength => unitSideLength;

    public Adaptor<Vector3> Vertices => new(vertices, gridCount);
    public Adaptor<Color> Colors => new(colors, gridCount);
    public Adaptor<Vector2> UVs => new(uvs, gridCount);

    /// <summary>
    /// Build the information in this instance out to a mesh
    /// </summary>
    public Mesh Build()
    {
        var mesh = new Mesh();

        // Set mesh data variables based on what has been constructed so far //
        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        // Calculate information from these parameters //
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Create a mesh builder configured for the provided settings.
    /// </summary>
    public ChunkMeshBuilder(int gridCount, float unitSideLength)
    {
        this.gridCount = gridCount;
        this.unitSideLength = unitSideLength;

        var vertCount = gridCount * gridCount + 2 * gridCount + 1;

        vertices = new Vector3[vertCount];
        colors = new Color[vertCount];
        uvs = new Vector2[vertCount];
        triangles = new int[gridCount * gridCount * 2 * 3];

        var triIndex = 0;
        var vertIndex = 0;

        var offsetForCenter = gridCount / 2 * unitSideLength;

        // Generate vertices //
        foreach (var (i, j) in Griderable.For(gridCount))
        {
            vertices[vertIndex++] = new Vector3(
                x: i * unitSideLength - offsetForCenter,
                y: 0,
                z: j * unitSideLength - offsetForCenter);
        }

        // Generate triangles //
        foreach (var (i, j) in Griderable.For(gridCount - 1))
        {
            var baseIndex = i * gridCount + j;

            triangles[triIndex++] = baseIndex;
            triangles[triIndex++] = baseIndex + gridCount;
            triangles[triIndex++] = baseIndex + 1;

            triangles[triIndex++] = baseIndex + gridCount;
            triangles[triIndex++] = baseIndex + gridCount + 1;
            triangles[triIndex++] = baseIndex + 1;
        }
    }
}
