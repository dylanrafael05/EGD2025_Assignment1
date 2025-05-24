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
    private readonly Dictionary<float, PersistentNoiseOctave> _octaves = new();

    /// <summary>
    /// Get the octave instance for the provided scale.
    /// </summary>
    public PersistentNoiseOctave Octave(float octave)
    {
        if (!_octaves.TryGetValue(octave, out var result))
        {
            result = new(octave);
            _octaves[octave] = result;
        }

        return result;
    }
}
