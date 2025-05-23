using System;
using UnityEngine;

public static class Generator
{
    public static Mesh BuildSimpleGridMesh()
    {
        var mesh = new Mesh();

        var gridCount = GeneratorSettings.Instance.gridCount;
        var gridSize = GeneratorSettings.Instance.unitSideLength;

        var vertices = new Vector3[gridCount * gridCount + 2 * gridCount + 1];
        var triangles = new int[gridCount * gridCount * 2 * 3];

        var triIndex = 0;
        var vertIndex = 0;

        var offsetForCenter = gridCount / 2 * gridSize;

        // Generate vertices //
        for (var i = 0; i < gridCount; i++)
        {
            for (var j = 0; j < gridCount; j++)
            {
                vertices[vertIndex++] = new Vector3(
                    x: i * gridSize - offsetForCenter,
                    y: 0,
                    z: j * gridSize - offsetForCenter);
            }
        }

        // Generate triangles //
        for (var i = 0; i < gridCount - 1; i++)
        {
            for (var j = 0; j < gridCount - 1; j++)
            {
                var baseIndex = i * gridCount + j;

                triangles[triIndex++] = baseIndex;
                triangles[triIndex++] = baseIndex + 1;
                triangles[triIndex++] = baseIndex + gridCount;

                triangles[triIndex++] = baseIndex + gridCount;
                triangles[triIndex++] = baseIndex + 1;
                triangles[triIndex++] = baseIndex + gridCount + 1;
            }
        }

        // Set up mesh //
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        // Calculate mesh bounds //
        mesh.RecalculateBounds();

        return mesh;
    }
}
