using System;
using Unity.Mathematics;
using UnityEngine;

public static class GenerationUtils
{
    public const float DEFAULT_HEIGHT = -1;

    private static bool HasWarnedForInvalidGroundHeight = false;

    /// <summary>
    /// A helper function to get the triangle information for the provided location.
    /// </summary>
    public static bool GetTriangleInformation(float2 location, out ChunkInstance chunk, out float2 localCoordinate, out int2x3 tri)
    {
        // Assign defaults to prevent compiler from yelling at us //
        tri = math.int2x3(0);
        localCoordinate = location;

        // Attempt to get the chunk //
        chunk = GeneratorManager.Instance.WorldToChunkInstance(location);

        if (chunk is null)
            return false;

        // Calculate the local and grid coordinates of the location //
        localCoordinate = location - (float2)chunk.Bounds.min;
        chunk.GroundMesher.WorldToGridPosition(localCoordinate, out var grid, out var gridFrac);

        // Compute the triangle coordinates //
        if (gridFrac.x < 1 - gridFrac.y)
        {
            tri = math.int2x3(
                grid + math.int2(0, 0),
                grid + math.int2(1, 0),
                grid + math.int2(0, 1)
            );
        }
        else
        {
            tri = math.int2x3(
                grid + math.int2(1, 0),
                grid + math.int2(1, 1),
                grid + math.int2(0, 1)
            );
        }

        return true;
    }

    /// <summary>
    /// A type defining the type of mesh being referred to in calls to
    /// <see cref="HeightAt(float2, ChunkMeshKind, bool)"/>.
    /// </summary>
    public enum ChunkMeshKind
    {
        Path,
        Ground
    }
    
    /// <summary>
    /// Check if the ground is a path at the provided location without
    /// performing the expensive perlin noise calculations.
    /// </summary>
    public static bool IsPathAt(float2 location)
        => PathHeightAt(location) > GroundHeightAt(location);
        
    /// <summary>
    /// Get the greatest height between the path and gronud meshes
    /// at the provided location.
    /// </summary>
    public static float StandHeightAt(float2 location, bool debug = false)
        => math.max(PathHeightAt(location, debug), GroundHeightAt(location, debug));

    /// <summary>
    /// Get the height of the path mesh at the provided location.
    /// </summary>
    public static float PathHeightAt(float2 location, bool debug = false)
        => HeightAt(location, ChunkMeshKind.Path, debug);

    /// <summary>
    /// Get the height of the ground mesh at the provided location.
    /// </summary>
    public static float GroundHeightAt(float2 location, bool debug = false)
        => HeightAt(location, ChunkMeshKind.Ground, debug);

    /// <summary>
    /// Get the height of the provided mesh type at the provided location.
    /// </summary>
    public static float HeightAt(float2 location, ChunkMeshKind mesh, bool debug = false)
    {
        var success = GetTriangleInformation(location, out var chunk, out var coordinate, out var triPositions);

        // Fall back to raycasting into the scene in the event that the ground has not yet
        // been generated. This is for testing purposes
        if (!success)
        {
            if (!HasWarnedForInvalidGroundHeight)
            {
                Debug.LogWarning($"The call to GroundHeightAt({location.x}, {location.y}) needs fallback to raycasting! This indicates either a problem, or that you are testing something.");
                HasWarnedForInvalidGroundHeight = true;
            }

            var rayOrigin = location.xxy;
            rayOrigin.y = -10f;

            var raycastRay = new Ray(rayOrigin, Vector3.up);

            if (Physics.Raycast(raycastRay, out var hit, 100f, Layers.GroundMask))
            {
                return hit.point.y;
            }

            return DEFAULT_HEIGHT;
        }

        // Get the triangle indices //
        var mesher = mesh switch
        {
            ChunkMeshKind.Path => chunk.PathMesher,
            ChunkMeshKind.Ground => chunk.GroundMesher,

            _ => throw new InvalidOperationException($"Unknown mesh kind {mesh}.")
        };

        float3x3 verts = math.float3x3(
            mesher.Vertices[triPositions[0]],
            mesher.Vertices[triPositions[1]],
            mesher.Vertices[triPositions[2]]);

        float2x3 vertBasePositions = math.float2x3(
            verts[0].xz,
            verts[1].xz,
            verts[2].xz);

        float3 vertHeights = math.float3(
            verts[0].y,
            verts[1].y,
            verts[2].y);

        // Calculate the barycentric coordinates //
        var bary = MathUtils.Barycentric(vertBasePositions, coordinate);
        var result = math.dot(bary, vertHeights);

        // Handle debugging //
        if (debug)
        {
            var min = chunk.Bounds.min.tofloat2().xxy;
            min.y = 0;

            Debug.DrawLine(verts[0] + min, verts[1] + min, Color.red);
            Debug.DrawLine(verts[1] + min, verts[2] + min, Color.red);
            Debug.DrawLine(verts[2] + min, verts[0] + min, Color.red);

            var cline = location.xxy;
            cline.y = result;

            Debug.DrawLine(cline - math.float3(0, 100, 0), cline + math.float3(0, 100, 0), Color.blue);

            Debug.DrawLine(cline - math.left() * 0.1f, cline + math.left() * 0.1f, Color.green);
            Debug.DrawLine(cline - math.up() * 0.1f, cline + math.up() * 0.1f, Color.green);
            Debug.DrawLine(cline - math.forward() * 0.1f, cline + math.forward() * 0.1f, Color.green);

            Debug.Log($"Barycentric: {bary}");
        }

        return result;
    }
}
