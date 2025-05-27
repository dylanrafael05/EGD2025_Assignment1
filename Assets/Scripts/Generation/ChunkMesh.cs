using System;
using Unity.Mathematics;
using UnityEngine;

public class ChunkMesher
{
    // Private fields //
    private readonly int gridCount;
    private readonly float unitSideLength;
    private readonly Vector3[] vertices;
    private readonly Color[] colors;
    private readonly Vector2[] uvs;
    private readonly int[] triangles;
    private readonly Mesh mesh;

    /// <summary>
    /// An adaptor around some array which provides multidimensional
    /// indexing, for the purposes of simple iteration over vertex information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Adaptor<T>
    {
        private readonly T[] array;
        private readonly ChunkMesher mesher;

        public Adaptor(T[] array, ChunkMesher mesher)
        {
            this.array = array;
            this.mesher = mesher;
        }

        public ref T this[int2 xy]
            => ref array[mesher.GridPositionToIndex(xy)];
        public ref T this[int x, int y]
            => ref this[math.int2(x, y)];
    }

    // Public APIs //
    public int GridCount => gridCount;
    public float UnitSideLength => unitSideLength;

    public Adaptor<Vector3> Vertices => new(vertices, this);
    public Adaptor<Color> Colors => new(colors, this);
    public Adaptor<Vector2> UVs => new(uvs, this);

    public Mesh Mesh => mesh;

    /// <summary>
    /// Helper method to get the underlying data index of the provided
    /// grid position, <paramref name="position"/>.
    /// </summary>
    public int GridPositionToIndex(int2 position)
        => position.x * (gridCount + 1) + position.y;

    /// <summary>
    /// Helper method to convert from world position (starting at 0, 0)
    /// to the indices within this mesh's grid as well as the fractional part
    /// between the grid positions.
    /// </summary>
    public void WorldToGridPosition(float2 position, out int2 gridPosition, out float2 gridFrac)
    {
        var divd = position / UnitSideLength;

        gridPosition = math.int2(divd);
        gridFrac = math.frac(divd);
    }

    /// <summary>
    /// Build the information in this instance out to the associated mesh instance
    /// </summary>
    public void UpdateMeshInfo()
    {
        // Set mesh data variables based on what has been constructed so far //
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);
        mesh.SetUVs(0, uvs);

        // Calculate information from these parameters //
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public void Reset()
    {
        var triIndex = 0;
        var vertIndex = 0;

        // Reset vertices //
        foreach (var (i, j) in Griderable.ForInclusive(gridCount))
        {
            vertices[vertIndex] = new Vector3(
                x: i * unitSideLength,
                y: 0,
                z: j * unitSideLength);

            uvs[vertIndex] = new Vector2(
                x: i / (gridCount - 1f),
                y: j / (gridCount - 1f));

            colors[vertIndex] = Color.white;

            vertIndex++;
        }

        // v triangles //
        foreach (var (i, j) in Griderable.For(gridCount))
        {
            var baseIndex = i * (gridCount + 1) + j;

            triangles[triIndex++] = baseIndex;
            triangles[triIndex++] = baseIndex + (gridCount + 1);
            triangles[triIndex++] = baseIndex + 1;

            triangles[triIndex++] = baseIndex + (gridCount + 1);
            triangles[triIndex++] = baseIndex + (gridCount + 1) + 1;
            triangles[triIndex++] = baseIndex + 1;
        }
    }

    /// <summary>
    /// Create a mesh builder configured for the provided settings.
    /// </summary>
    public ChunkMesher(int gridCount, float unitSideLength)
    {
        this.gridCount = gridCount;
        this.unitSideLength = unitSideLength;

        var vertCount = gridCount * gridCount + 2 * gridCount + 1;

        vertices = new Vector3[vertCount];
        colors = new Color[vertCount];
        uvs = new Vector2[vertCount];
        triangles = new int[gridCount * gridCount * 2 * 3];

        mesh = new();

        Reset();
    }
}
