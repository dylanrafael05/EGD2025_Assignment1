using System.Collections.Generic;
using Unity.Mathematics;
/// <summary>
/// A wrapper around <see cref="PersistentPerlinNoiseOctave"/> which facilitates
/// simple construction of fractal noise.
/// 
/// <para>
/// Note that using runtime-calculated (unfixed) octave sizes will lead to 
/// a mass increase in memory usage, and potential errors, and is thus unadvised.
/// </para>
/// </summary>
public class PersistentPerlinNoise
{
    private readonly Dictionary<float, PersistentPerlinNoiseOctave> octaves = new();

    /// <summary>
    /// Calls <see cref="PersistentPerlinNoiseOctave.UnloadChunk(ChunkID)"/> on all created octaves.
    /// </summary>
    public void UnloadChunk(ChunkID id)
    {
        foreach (var octave in octaves.Values)
            octave.UnloadChunk(id);
    }

    /// <summary>
    /// Get the octave instance for the provided scale.
    /// </summary>
    public PersistentPerlinNoiseOctave Octave(float octave)
    {
        if (!octaves.TryGetValue(octave, out var result))
        {
            result = new(octave);
            octaves[octave] = result;
        }

        return result;
    }

    /// <summary>
    /// Evaluate the provided octaves at the provided position.
    /// </summary>
    public float Get(ChunkID id, float2 pos, bool normalize = false, params OctaveInformation[] octaves)
    {
        float result = 0;
        float weightSum = 0;

        foreach (var octave in octaves)
        {
            result += Octave(octave.scale).Get(id, pos) * octave.amplitude;
            weightSum += octave.amplitude;
        }

        if (!normalize)
            weightSum = 1;

        return result / weightSum;
    }
}
