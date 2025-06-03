
/// <summary>
/// A helper class to permit unloading chunks through one large interface.
/// Instances of <see cref="IChunkCacher"/> should call <see cref="Register(IChunkCacher)"/>
/// on construction, and <see cref="Unregister(IChunkCacher)"/> when it is 
/// no longer needed.
/// </summary>
public static class ChunkCachers
{
    /// <summary>
    /// An interface representing objects which store data corresponding
    /// to chunk IDs that can be unloaded at will.
    /// </summary>
    public interface IChunkCacher
    {
        void UnloadChunk(ChunkID id);
    }

    private static ConcurrentSet<IChunkCacher> instances = new();

    /// <summary>
    /// Register a chunk cacher with the global set of cachers.
    /// </summary>
    public static void Register(IChunkCacher instance)
    {
        instances.Add(instance);
    }

    /// <summary>
    /// Unregister a chunk cacher with the global set.
    /// This will permit its collection by the GC.
    /// </summary>
    public static void Unregister(IChunkCacher instance)
    {
        instances.Remove(instance);
    }

    /// <summary>
    /// Unload chunk data from all registered cachers.
    /// </summary>
    public static void UnloadChunk(ChunkID id)
    {
        foreach (var instance in instances)
        {
            instance.UnloadChunk(id);
        }
    }
}
