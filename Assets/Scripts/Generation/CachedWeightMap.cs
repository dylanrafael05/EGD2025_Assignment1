using System;
using UnityEngine;
using Unity.Mathematics;

/// <summary>
/// A class which facilitates storing and recalling float values interpolated
/// over a two-dimensional surface, associated with chunks.
/// </summary>
public class CachedWeightMap : ChunkCachers.IChunkCacher
{
    private readonly CachedValueStore<ChunkID, int2, float> cached;
    private readonly float scale;

    public CachedWeightMap(float scale, float defaultValue)
    {
        this.scale = scale;
        cached = new(_ => defaultValue);

        ChunkCachers.Register(this);
    }

    /// <summary>
    /// Unload all cached information associated with the provided chunk.
    /// </summary>
    public void UnloadChunk(ChunkID id)
    {
        cached.UnloadID(id);
    }

    /// <summary>
    /// Get the interpolated value at the provided location, accessed from
    /// the provided chunk.
    /// </summary>
    public float Get(ChunkID chunk, float2 location)
    {
        location *= scale;

        var corners = MathUtils.Corners(location);
        var values = math.float4(
            cached.Get(chunk, (int2)corners[0]),
            cached.Get(chunk, (int2)corners[1]),
            cached.Get(chunk, (int2)corners[2]),
            cached.Get(chunk, (int2)corners[3])
        );

        return MathUtils.SampleCorners(math.frac(location), values);
    }

    /// <summary>
    /// Update the nearest value to the provided location, if such a value
    /// exists.
    /// </summary>
    public bool UpdateNearestIfExists(float2 location, Func<float, float> updator)
    {
        location *= scale;
        location = math.round(location);

        return cached.UpdateIfExists((int2)location, updator);
    }

    /// <summary>
    /// Update the nearest value to the provided location, if such a value
    /// exists, currying the provided argument into the function call
    /// to avoid a memory allocation.
    /// </summary>
    public bool UpdateNearestIfExists<T>(float2 location, Func<float, T, float> updator, T genarg)
    {
        location *= scale;
        location = math.round(location);

        return cached.UpdateIfExists((int2)location, updator, genarg);
    }
}
