using Unity.Mathematics;

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

        return math.float2x4(
            xf + math.float2(0, 0),
            xf + math.float2(0, 1),
            xf + math.float2(1, 1),
            xf + math.float2(1, 0));
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
