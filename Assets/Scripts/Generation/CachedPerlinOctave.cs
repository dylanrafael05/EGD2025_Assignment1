using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A class which facilitates perlin noise generation with persistence that can
/// be removed at runtime.
/// </summary>
public class CachedPerlinNoise : ChunkCachers.IChunkCacher
{
    /// <summary>
    /// A utility class representing a single octave that can be
    /// serialized and changed in the editor.
    /// </summary>
    [Serializable]
    public class Octave
    {
        [SerializeField] private float scale = 1;
        [SerializeField] private float amplitude = 1;

        public float Scale => scale;
        public float Amplitude => amplitude;

        private CachedPerlinNoise instance;
        public CachedPerlinNoise GetInstance()
        {
            if (instance == null
            || instance.octaveScale != scale
            || instance.octaveAmplitude != amplitude)
            {
                lock (this)
                {
                    if (instance != null
                    && instance.octaveScale == scale
                    && instance.octaveAmplitude == amplitude)
                        return instance;

                    if (instance != null)
                        ChunkCachers.Unregister(instance);

                    instance = CachedPerlinNoise.OfScale(scale, amplitude);
                }
            }

            return instance;
        }

        /// <summary>
        /// Sample this octave instance.
        /// </summary>
        public float Get(ChunkID chunk, float2 pos)
            => GetInstance().Get(chunk, pos);

        /// <summary>
        /// Unload the gradients for the provided chunk.
        /// </summary>
        public void UnloadChunk(ChunkID chunk)
            => GetInstance().UnloadChunk(chunk);
    }

    /// <summary>
    /// A utility class wrapping a sample of "fractal" noise,
    /// which is comprised of individual octaves.
    /// </summary>
    [Serializable]
    public class Fractal
    {
        [SerializeField] private List<Octave> octaves;
        private float totalAmplitude = float.NaN;

        public IReadOnlyList<Octave> Octaves => octaves;
        public float TotalAmplitude
        {
            get
            {
                if (float.IsNaN(totalAmplitude))
                {
                    totalAmplitude = 0;
                    foreach (var octave in octaves)
                    {
                        totalAmplitude += octave.Amplitude;
                    }
                }

                return totalAmplitude;
            }
        }
        

        /// <summary>
        /// Sample this fractal noise instance.
        /// </summary>
        public float Get(ChunkID chunk, float2 pos, bool normalize = false)
        {
            float result = 0;
            float weightSum = 0;

            foreach (var octave in octaves)
            {
                result += octave.Get(chunk, pos);
                weightSum += octave.Amplitude;
            }

            if (!normalize)
                weightSum = 1;

            return result / weightSum;
        }

        /// <summary>
        /// Unload the gradients for the provided chunk.
        /// </summary>
        public void UnloadChunk(ChunkID chunk)
        {
            foreach (var octave in octaves)
                octave.UnloadChunk(chunk);
        }
    }

    /// <summary>
    /// Create a new instance with the provided octave scale.
    /// Non-positive, infinite, and undefined scales will result in an <see cref="ArgumentException"/>.
    /// </summary>
    public CachedPerlinNoise(float scale, float amplitude)
    {
        if (scale <= 0 || float.IsNaN(scale) || !float.IsFinite(scale))
            throw new ArgumentException("Octave scale must be positive and non-nan", nameof(scale));

        octaveScale = scale;
        octaveAmplitude = amplitude;

        ChunkCachers.Register(this);
    }

    /// <summary>
    /// Helper method which names the constructor.
    /// </summary>
    public static CachedPerlinNoise OfScale(float scale, float amplitude = 1)
        => new(scale, amplitude);

    // Fields //
    private readonly float octaveScale;
    private readonly float octaveAmplitude;
    private readonly CachedValueStore<ChunkID, int2, float2> cached = new(
        _ => ThreadSafeRandom.Get().NextFloat2Direction());

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

        var corners = MathUtils.Corners(position);
        var grads = math.float2x4(
            cached.Get(chunk, (int2)corners[0]),
            cached.Get(chunk, (int2)corners[1]),
            cached.Get(chunk, (int2)corners[2]),
            cached.Get(chunk, (int2)corners[3])
        );

        // Finally, use our gradients to calculate the perlin noise results //
        return MathUtils.PerlinNoiseFromGrads(math.frac(position), grads) * octaveAmplitude;
    }
}
