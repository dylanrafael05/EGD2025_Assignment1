using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

/// <summary>
/// A utility class to facilitate iteration over a two-dimensional grid of integers
/// without needing to nest loops.
/// </summary>
public readonly struct Griderable : IEnumerable<int2>
{
    // Grid information //
    private readonly int2 min;
    private readonly int2 max;

    // Construction //
    private Griderable(int2 min, int2 max)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// Create a Griderable which will iterate all points between
    /// <paramref name="min"/> (inclusive) and <paramref name="max"/>
    /// (exclusive).
    /// </summary>
    public static Griderable For(int2 min, int2 max)
        => new(min, max);

    /// <summary>
    /// Create a Griderable which will iterate all points between
    /// <c>[0, 0]</c> (inclusive) and <paramref name="max"/>
    /// (exclusive).
    /// </summary>
    public static Griderable For(int2 max)
        => new(int2.zero, max);

    // Enumerator //
    public Enumerator GetEnumerator()
        => new(this);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    IEnumerator<int2> IEnumerable<int2>.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// The helper struct responsible for the actual iteration of values.
    /// </summary>
    public struct Enumerator : IEnumerator<int2>
    {
        private readonly Griderable grid;
        private int2 value;

        /// <summary>
        /// Construct an enumerator around the provided Griderable.
        /// </summary>
        public Enumerator(Griderable grid)
        {
            this.grid = grid;
            this.value = default;

            Reset();
        }

        public readonly int2 Current => value;
        readonly object IEnumerator.Current => value;

        public readonly void Dispose()
        { }

        public bool MoveNext()
        {
            // Try to advance in the x direction first //
            value.x++;

            // If we have overshot, return to the beginning of the //
            // next row.                                           //
            if (value.x >= grid.max.x)
            {
                value.y++;
                value.x = grid.min.x;
            }

            // Report we have finished iteration if we are on the row //
            // beyond the last one.                                   //
            return value.y < grid.max.y;
        }

        public void Reset()
        {
            // Move one unit before the begin in the x direction to follow //
            // convention that MoveNext() must be called at least once     //
            value = grid.min - math.int2(1, 0);
        }
    }
}
