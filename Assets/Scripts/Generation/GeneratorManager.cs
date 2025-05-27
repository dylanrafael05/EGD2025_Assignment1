using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class GeneratorManager : MonoBehaviour
{
    public static GeneratorManager Instance { get; private set; }

    // Editor variables //
    [Header("Settings")]
    [SerializeField] private ChunkInstance chunkPrefab;
    [SerializeField] private Transform generationCenter;
    [SerializeField] private float generationRadius;

    [Header("Grid Settings")]
    public int gridCount;
    public float gridSideLength;

    [Header("Generation Settings")]
    [SerializeField] private OctaveInformation[] heightOctaves;
    [SerializeField] private ScenePropPlacer[] propPlacers;
    [SerializeField] private float groundColorMinHeight;
    [SerializeField] private float groundColorMaxHeight;
    [SerializeField] private Gradient groundColor;

    // Implementation variables //
    private InstancePool<ChunkInstance> chunkPool;

    private readonly Dictionary<int2, ChunkInstance> loadedChunks = new();

    public float UnitSideLength => gridSideLength / gridCount;
    public int GenerationRadiusInChunks => Mathf.CeilToInt(generationRadius / gridSideLength);

    /// <summary>
    /// Generate a new chunk at the given location.
    /// </summary>
    private void Generate(int2 location)
    {
        // Get the instance of the chunk and set up its ID and location //
        var chunk = chunkPool.Get();

        chunk.ID = ChunkID.Unique();
        chunk.Position = location;

        foreach (var pos in Griderable.ForInclusive(gridCount))
        {
            ref var vertex = ref chunk.Mesher.Vertices[pos];

            // Apply height offsets for every octave calculated //
            foreach (var octave in heightOctaves)
            {
                var vpos = vertex.tofloat3().xz;
                vpos += (float2)location * gridSideLength;

                vertex.y += (GenerationUtils.Noise.Octave(octave.scale).Get(chunk.ID, vpos) * 2 - 1)
                    * octave.amplitude;
            }

            // Populate vertex colors based on vertex height //
            chunk.Mesher.Colors[pos] = groundColor.Evaluate(Mathf.InverseLerp(groundColorMinHeight, groundColorMaxHeight, vertex.y));
        }

        // Finalize the mesh and update the instance location //
        chunk.Mesher.UpdateMeshInfo();
        chunk.transform.position = new(
            x: location.x * gridSideLength,
            y: 0,
            z: location.y * gridSideLength
        );

        chunk.Bounds = Rect.MinMaxRect(
            chunk.transform.position.x,
            chunk.transform.position.z,
            chunk.transform.position.x + gridSideLength,
            chunk.transform.position.z + gridSideLength);

        // Place props //
        foreach (var placer in propPlacers)
            placer.PlaceInChunk(chunk);

        // Cache this chunk //
        loadedChunks.Add(location, chunk);
    }

    void Awake()
    {
        // Setup the singleton //
        Instance = this;

        // Setup the pool for the chunks //
        chunkPool = InstancePool.OfPrefab(chunkPrefab, transform);
    }

    bool ChunkShouldCull(int2 chunk, int2 center)
        => math.csum(math.abs(chunk - center)) > GenerationRadiusInChunks;

    void Update()
    {
        var center = (int2)math.floor(generationCenter.position.tofloat3().xz / gridSideLength);

        // Unload all chunks that are too far away //
        // This should happen first so new chunks can use those unloaded here //
        var chunksToUnload = ListPool<int2>.Get();

        foreach (var chunk in loadedChunks.Keys)
        {
            if (ChunkShouldCull(chunk, center))
            {
                chunksToUnload.Add(chunk);
            }
        }

        foreach (var chunk in chunksToUnload)
        {
            loadedChunks.Remove(chunk, out var instance);
            GenerationUtils.Noise.UnloadChunk(instance.ID);
            chunkPool.Release(instance);
        }

        ListPool<int2>.Release(chunksToUnload);

        // Generate new chunks as necessary //
        foreach (var offset in Griderable.ForInclusive(-GenerationRadiusInChunks, GenerationRadiusInChunks))
        {
            var chunk = offset + center;

            if (ChunkShouldCull(chunk, center))
                continue;

            if (!loadedChunks.ContainsKey(chunk))
                Generate(chunk);
        }
    }
}
