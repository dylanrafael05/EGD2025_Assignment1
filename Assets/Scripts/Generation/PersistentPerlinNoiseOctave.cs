using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A class which facilitates perlin noise generation with persistence that can
/// be removed at runtime.
/// </summary>
public class PersistentPerlinNoiseOctave
{
    /// <summary>
    /// Create a new instance with the provided octave scale.
    /// Non-positive, infinite, and undefined scales will result in an <see cref="ArgumentException"/>.
    /// </summary>
    public PersistentPerlinNoiseOctave(float scale)
    {
        if (scale <= 0 || float.IsNaN(scale) || !float.IsFinite(scale))
            throw new ArgumentException("Octave scale must be positive and non-nan", nameof(scale));

        octaveScale = scale;
    }

    // Fields //
    private readonly float octaveScale;
    private readonly CachedValueStore<ChunkID, float2, float2> cached = new(
        _ => UnityEngine.Random.insideUnitCircle.normalized);

    /// <summary>
    /// Remove all gradient data associated with the provided chunk ID,
    /// allowing new data to be populated later (assuming no other chunks
    /// depend on it).
    /// </summary>
    public void UnloadChunk(ChunkID chunk)
        => cached.UnloadID(chunk);

    /// <summary>
    /// Evaluate the perlin noise octave at the provided position,
    /// using cached gradients if they exist.
    /// </summary>
    public float Get(ChunkID chunk, float2 position)
    {
        // Multiply our position by our octave scale to facilitate //
        // easier manipulation of fractal noise                    //
        position *= octaveScale;
        // position.x *= 100;

        //! SAFETY !//
        // Since .Corners uses floor and addition to calculate corners
        // based on the position provided, results *should* be deterministic
        // for not only identical positions, but also for *similar* positions
        // (unless an integer boundary is crossed, but the perlin noise algorithm
        // already handles this elegantly so this is of no concern).
        var corners = MathUtils.Corners(position);
        var grads = math.float2x4(
            cached.Get(chunk, corners[0]),
            cached.Get(chunk, corners[1]),
            cached.Get(chunk, corners[2]),
            cached.Get(chunk, corners[3])
        );

        // Finally, use our gradients to calculate the perlin noise results //
        return MathUtils.PerlinNoiseFromGrads(math.frac(position), grads);
    }
}
