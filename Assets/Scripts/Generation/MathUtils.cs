using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A helper class containing useful methods for mathematics, built on the
/// architecture and conventions provided by the <see cref="Unity.Mathematics"/>
/// namespace.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Compute a smooth interpolation kernel (ranged between zero and one)
    /// for the provided two-dimensional value.
    /// </summary>
    public static float2 Smootherstep(float2 x)
    {
        return x * x * x * (x * (6.0f * x - 15.0f) + 10.0f);
    }

    /// <summary>
    /// Equivalent to <c>a.x * b.y - b.x * a.y</c>.
    /// </summary>
    public static float CrossDot(float2 a, float2 b)
    {
        return a.x * b.y - b.x * a.y;
    }

    /// <summary>
    /// Get the barycentric coordinates of the provided position <paramref name="r"/>
    /// within the triangle defined by the three rows in <paramref name="tri"/>.
    /// </summary>
    public static float3 Barycentric(float2x3 tri, float2 r)
    {
        // Get the denominator //
        var d = CrossDot(tri[0] - tri[2], tri[1] - tri[2]);

        var l1 = CrossDot(r - tri[2], tri[1] - tri[2]) / d;
        var l2 = CrossDot(r - tri[2], tri[2] - tri[0]) / d;

        return math.float3(l1, l2, 1 - l1 - l2);
    }

    /// <summary>
    /// A matrix whose rows are the permutations of two binary values, 
    /// defined in the order
    /// <c>{[0] = (0, 0), [1] = (0, 1), [2] = (1, 1), [3] = (1, 0)}</c>.
    /// </summary>
    public static float2x4 BinaryPairs => math.float2x4(
        math.float2(0, 0),
        math.float2(0, 1),
        math.float2(1, 1),
        math.float2(1, 0)
    );

    /// <summary>
    /// A matrix whose rows are the permutations of the two signs,
    /// defined in the same order as is <see cref="BinaryPairs"/>.
    /// </summary>
    public static float2x4 SignPairs => BinaryPairs * 2 - 1;

    /// <summary>
    /// Compute a matrix whose rows are the corners of the unit-size bounding square
    /// aligned to integer coordinates within which <paramref name="x"/> resides. 
    /// 
    /// The order of resultant corners is always <c>[t00, t01, t11, t10]</c>, where
    /// <c>tXY</c> represents the top-left corner <c>t</c> of the box, summed with the
    /// vector <c>[X, Y]</c>.
    /// </summary>
    public static float2x4 Corners(float2 x)
    {
        var xf = math.floor(x);

        return math.float2x4(xf, xf, xf, xf) + BinaryPairs;
    }

    /// <summary>
    /// Perform bilinear sampling at the provided position <paramref name="x"/> between the 
    /// provided <paramref name="corners"/> values (ordered as defined by <see cref="Corners(float2)"/>)
    /// </summary>
    public static float SampleCorners(float2 x, float4 corners)
    {
        float2 f = Smootherstep(math.frac(x));

        //? Does it make sense that the corners are passed this way ?//
        return math.lerp(
            math.lerp(corners[0], corners[1], f.y),
            math.lerp(corners[3], corners[2], f.y),
            f.x);
    }

    /// <summary>
    /// Compute the result of perlin noise at the normalized position <paramref name="x"/>
    /// with the corner gradients (in the order defined by <see cref="Corners(float2)"/>)
    /// as defined by <paramref name="grads"/>.
    /// </summary>
    public static float PerlinNoiseFromGrads(float2 x, float2x4 grads)
    {
        var corners = math.float4(
            math.dot(grads.c0, x - math.float2(0, 0)),
            math.dot(grads.c1, x - math.float2(0, 1)),
            math.dot(grads.c2, x - math.float2(1, 1)),
            math.dot(grads.c3, x - math.float2(1, 0)));

        return SampleCorners(x, corners) * 0.5f + 0.5f;
    }
}
