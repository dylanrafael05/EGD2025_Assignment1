using Unity.Mathematics;

/// <summary>
/// A helper class which provides extension method implementations of the
/// <c>value.Deconstruct(out ...)</c> paradigm for the vector-like primitives
/// defined by <see cref="Unity.Mathematics"/>.
/// </summary>
public static class MathDeconstructors
{
    public static void Deconstruct(this int2 xy, out int x, out int y)
    {
        x = xy.x;
        y = xy.y;
    }

    public static void Deconstruct(this float2 xy, out float x, out float y)
    {
        x = xy.x;
        y = xy.y;
    }
}
