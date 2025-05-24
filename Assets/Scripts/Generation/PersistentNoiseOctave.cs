using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Pool;
/// <summary>
/// A class which facilitates perlin noise generation with persistence that can
/// be removed at runtime.
/// </summary>
public class PersistentNoiseOctave
{
    /// <summary>
    /// A helper struct to store the information needed
    /// by a single gradient in the cache.
    /// </summary>
    private struct GradValue
    {
        public int useCount;
        public float2 value;
    }

    /// <summary>
    /// Create a new instance with the provided octave scale.
    /// Non-positive, infinite, and undefined scales will result in an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="scale"></param>
    public PersistentNoiseOctave(float scale)
    {
        if (scale <= 0 || float.IsNaN(scale) || !float.IsFinite(scale))
            throw new ArgumentException("Octave scale must be positive and non-nan", nameof(scale));

        octaveScale = scale;
        rand = new(math.asuint(scale));
    }

    // Fields //
    private readonly float octaveScale;
    private readonly Unity.Mathematics.Random rand;
    // We store our random here ^ so we dont recreate it every time we need it //

    private readonly Dictionary<ChunkID, HashSet<float2>> chunkToUsedGrads = new();
    private readonly Dictionary<float2, GradValue> grads = new();

    /// <summary>
    /// A helper method to get (or initialize and cache) the set of 
    /// gradients (by position) that are 'used' by the provided chunk id,
    /// <paramref name="chunk"/>.
    /// </summary>
    private HashSet<float2> GetUsedGrads(ChunkID chunk)
    {
        if (!chunkToUsedGrads.TryGetValue(chunk, out var used))
        {
            used = HashSetPool<float2>.Get();
            chunkToUsedGrads.Add(chunk, used);
        }

        return used;
    }

    /// <summary>
    /// Remove all gradient data associated with the provided chunk ID,
    /// allowing new data to be populated later (assuming no other chunks
    /// depend on it).
    /// </summary>
    public void UnloadChunk(ChunkID chunk)
    {
        // For every gradient used by this chunk id //
        var used = GetUsedGrads(chunk);

        foreach (var grad in used)
        {
            // Remove one use from its counter //
            var value = grads[grad];
            value.useCount--;

            // And remove it if it is now unused //
            if (value.useCount == 0)
            {
                grads.Remove(grad);
            }
            else
            {
                grads[grad] = value;
            }
        }

        // Then free the association set //
        chunkToUsedGrads.Remove(chunk);
        HashSetPool<float2>.Release(used);
    }

    /// <summary>
    /// Get a perlin noise gradient for the corner position <paramref name="xy"/>, associated with
    /// the provided chunk id, <paramref name="chunk"/>.
    /// 
    /// <para>
    /// Subsequent calls to this function with <b>identical</b> values of <paramref name="xy"/> will
    /// return the same value, unless <see cref="UnloadChunk(ChunkID)"/> is called and unloads
    /// the persistent gradient information. Because calls to this function depend on <b>identical</b>
    /// inputs for floating point numbers, calculations of these values should be performed using
    /// <i>ideally exactly one function</i>.
    /// </para>
    /// </summary>
    public float2 Grad(ChunkID chunk, float2 xy)
    {
        // Find the cached gradients //
        var used = GetUsedGrads(chunk);
        GradValue grad;

        if (grads.ContainsKey(xy))
        {
            // If the gradient exists, ensure the provided chunk is //
            // marked as using it                                   //
            grad = grads[xy];

            if (!used.Contains(xy))
            {
                used.Add(xy);
                grad.useCount++;
            }
        }
        else
        {
            // Otherwise, generate a new gradient //
            grad = new GradValue
            {
                useCount = 1,
                value = rand.NextFloat2Direction()
            };

            used.Add(xy);
            grads[xy] = grad;
        }

        return grad.value;
    }

    /// <summary>
    /// Evaluate the perlin noise octave at the provided position,
    /// using cached gradients if they exist.
    /// </summary>
    public float Get(ChunkID chunk, float2 position)
    {
        // Multiply our position by our octave scale to facilitate //
        // easier manipulation of fractal noise                    //
        position *= octaveScale;

        //! SAFETY !//
        // Since .Corners uses floor and addition to calculate corners
        // based on the position provided, results *should* be deterministic
        // for not only identical positions, but also for *similar* positions
        // (unless an integer boundary is crossed, but the perlin noise algorithm
        // already handles this elegantly so this is of no concern).
        var corners = MathUtils.Corners(position);
        var grads = math.float2x4(
            Grad(chunk, corners[0]),
            Grad(chunk, corners[1]),
            Grad(chunk, corners[2]),
            Grad(chunk, corners[3])
        );

        // Finally, use our gradients to calculate the perlin noise results //
        return MathUtils.PerlinNoiseFromGrads(math.frac(position), grads);
    }
}
