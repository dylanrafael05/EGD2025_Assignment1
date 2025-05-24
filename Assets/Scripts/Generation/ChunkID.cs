using System;

/// <summary>
/// A bare-bones wrapper around an integer used for identifying
/// some unique value.
/// </summary>
public readonly struct ChunkID : IEquatable<ChunkID>
{
    // Private implementation //
    private readonly uint id;

    private ChunkID(uint id)
    {
        this.id = id;
    }

    // Public facing API for creation of values //
    private static uint nextId = 0;

    public static ChunkID Unique()
        => new(nextId++);

    // Equality overrides and utilities //
    public override bool Equals(object other)
        => other is ChunkID chunkID && Equals(chunkID);

    public bool Equals(ChunkID other)
        => id == other.id;

    public override int GetHashCode()
        => id.GetHashCode();

    public override string ToString()
        => "ChunkID{" + id + "}";

    public static bool operator ==(ChunkID l, ChunkID r)
        => l.Equals(r);
    public static bool operator !=(ChunkID l, ChunkID r)
        => !l.Equals(r);
}
