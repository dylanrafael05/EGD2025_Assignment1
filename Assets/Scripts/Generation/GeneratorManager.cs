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
    [SerializeField] private ChunkRenderer chunkPrefab;
    [SerializeField] private Transform generationCenter;
    [SerializeField] private float generationRadius;

    [Header("Grid Settings")]
    public int gridCount;
    public float gridSideLength;

    [Header("Generation Settings")]
    [SerializeField] private OctaveInformation[] heightOctaves;
    [SerializeField] private float groundColorMinHeight;
    [SerializeField] private float groundColorMaxHeight;
    [SerializeField] private Gradient groundColor;

    // Implementation variables //
    private PersistentNoise noise = new();
    private ObjectPool<ChunkRenderer> chunkPool;
    private readonly Dictionary<int2, ChunkRenderer> loadedChunks = new();

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

        // Construct the mesh for this chunk //
        var builder = new ChunkMeshBuilder(gridCount, UnitSideLength);

        foreach (var pos in Griderable.ForInclusive(gridCount))
        {
            ref var vertex = ref builder.Vertices[pos];

            // Apply height offsets for every octave calculated //
            foreach (var octave in heightOctaves)
            {
                var vpos = vertex.tofloat3().xz;
                vpos += (float2)location * gridSideLength;

                vertex.y += (noise.Octave(octave.scale).Get(chunk.ID, vpos) * 2 - 1)
                    * octave.amplitude;
            }

            // Populate vertex colors based on vertex height //
            builder.Colors[pos] = groundColor.Evaluate(Mathf.InverseLerp(groundColorMinHeight, groundColorMaxHeight, vertex.y));
        }

        // Finalize the mesh and update the instance location //
        chunk.SetMesh(builder.Build());
        chunk.transform.position = new(
            x: location.x * gridSideLength,
            y: 0,
            z: location.y * gridSideLength
        );

        // Cache this chunk //
        loadedChunks.Add(location, chunk);
    }

    void Awake()
    {
        // Setup the singleton //
        Instance = this;

        // Setup the pool for the chunks //
        chunkPool = new(
            createFunc: () => GameObject.Instantiate(chunkPrefab, Instance.transform),
            actionOnGet: r => r.Activate(),
            actionOnRelease: r => r.Deactivate()
        );
    }

    bool ChunkShouldCull(int2 chunk, int2 center)
        => math.csum(math.abs(chunk - center)) > GenerationRadiusInChunks;

    void Update()
    {
        var center = (int2)math.floor(generationCenter.position.tofloat3().xz / gridSideLength + 0.5f);

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
            noise.UnloadChunk(instance.ID);
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
