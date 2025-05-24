using System.Collections.Generic;
/// <summary>
/// A wrapper around <see cref="PersistentNoiseOctave"/> which facilitates
/// simple construction of fractal noise.
/// 
/// <para>
/// Note that using runtime-calculated (unfixed) octave sizes will lead to 
/// a mass increase in memory usage, and potential errors, and is thus unadvised.
/// </para>
/// </summary>
public class PersistentNoise
{
    private readonly Dictionary<float, PersistentNoiseOctave> octaves = new();

    /// <summary>
    /// Calls <see cref="PersistentNoiseOctave.UnloadChunk(ChunkID)"/> on all created octaves.
    /// </summary>
    public void UnloadChunk(ChunkID id)
    {
        foreach (var octave in octaves.Values)
            octave.UnloadChunk(id);
    }

    /// <summary>
    /// Get the octave instance for the provided scale.
    /// </summary>
    public PersistentNoiseOctave Octave(float octave)
    {
        if (!octaves.TryGetValue(octave, out var result))
        {
            result = new(octave);
            octaves[octave] = result;
        }

        return result;
    }
}
