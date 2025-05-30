using Unity.Mathematics;
using UnityEngine;

public static class GenerationUtils
{
    public static PersistentPerlinNoise Noise { get; } = new();

    public const float DEFAULT_HEIGHT = -1;

    private static bool HasWarnedForInvalidGroundHeight = false;

    public static float GroundHeightAt(float2 location, bool debug = false)
    {
        var chunk = GeneratorManager.Instance.WorldToChunkInstance(location);

        // Fall back to raycasting into the scene in the event that the ground has not yet
        // been generated. This is for testing purposes
        if (chunk is null)
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
        var coordinate = location - (float2)chunk.Bounds.min;
        chunk.GroundMesher.WorldToGridPosition(coordinate, out var grid, out var gridFrac);

        int2x3 triPositions;

        if (gridFrac.x < 1 - gridFrac.y)
        {
            triPositions = math.int2x3(
                grid + math.int2(0, 0),
                grid + math.int2(1, 0),
                grid + math.int2(0, 1)
            );
        }
        else
        {
            triPositions = math.int2x3(
                grid + math.int2(1, 0),
                grid + math.int2(1, 1),
                grid + math.int2(0, 1)
            );
        }

        float3x3 verts = math.float3x3(
            chunk.GroundMesher.Vertices[triPositions[0]],
            chunk.GroundMesher.Vertices[triPositions[1]],
            chunk.GroundMesher.Vertices[triPositions[2]]);

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
