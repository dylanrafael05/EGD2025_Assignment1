using Unity.Mathematics;
using UnityEngine;

public static class GenerationUtils
{
    public static PersistentPerlinNoise Noise { get; } = new();

    public static float GroundHeightAt(float2 coordinate)
    {
        var rayOrigin = coordinate.xxy;
        rayOrigin.y = -10f;

        var raycastRay = new Ray(rayOrigin, Vector3.up);

        if (Physics.Raycast(raycastRay, out var hit, 100f, Layers.GroundMask))
        {
            return hit.point.y;
        }

        return float.NaN;
    }
}
