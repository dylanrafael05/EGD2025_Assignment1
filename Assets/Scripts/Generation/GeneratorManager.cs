using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private CachedPerlinNoise.Fractal heightNoise;
    [SerializeField] private CachedPerlinNoise.Fractal forestNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathFirstNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathSecondNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathMixNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathSubtractNoise;

    [SerializeField] private ScenePropPlacer[] propPlacers;
    [SerializeField] private float groundColorMinHeight;
    [SerializeField] private float groundColorMaxHeight;
    [SerializeField] private Color pathColor;
    [SerializeField] private Gradient groundColor;
    [SerializeField] private float forestAffectGridSize = 3f;
    [SerializeField] private float forestDecayOnKill = 0.5f;
    [SerializeField] private float pathSearchRadius = 0.05f;
    [SerializeField] private float pathIndent = 0.1f;
    [SerializeField] private float pathSubEffect = 0.1f;

    // Implementation variables //
    private InstancePool<ChunkInstance> chunkPool;

    private readonly HashSet<int2> generatingChunks = new();
    private readonly Dictionary<int2, ChunkInstance> loadedChunks = new();
    private CachedWeightMap forestDampenWeight;

    public float UnitSideLength => gridSideLength / gridCount;
    public int GenerationRadiusInChunks => Mathf.CeilToInt(generationRadius / gridSideLength);

    public int2 WorldToChunkPosition(float2 position)
        => (int2)math.floor(position / gridSideLength);

    public ChunkInstance WorldToChunkInstance(float2 position)
        => loadedChunks.GetValueOrDefault(WorldToChunkPosition(position), null);

    public void KillForestAt(float2 pos)
        => Debug.Assert(forestDampenWeight.UpdateNearestIfExists(
            pos,
            (x, y) => Mathf.Clamp01(x * y),
            forestDecayOnKill));

    public float CalcForestDampen(ChunkID id, float2 pos)
        => forestDampenWeight.Get(id, pos);

    public float CalcForestChance(ChunkID id, float2 pos)
        => forestNoise.Get(id, pos, true)
         * math.pow(1 - CalcTerrainHeight(id, pos, normalize: true), 0.5f)
         * forestDampenWeight.Get(id, pos);

    public float CalcPathHeightmapAtLocation(ChunkID id, float2 pos)
        => math.lerp(
            pathFirstNoise.Get(id, pos, true) * 2 - 1,
            pathSecondNoise.Get(id, pos, true) * 2 - 1,
            pathMixNoise.Get(id, pos, true)
        ) - pathSubtractNoise.Get(id, pos, true) * pathSubEffect;

    public bool CalcIsPath(ChunkID id, float2 pos)
    {
        // Optimization :: heightmap values that are too far away from zero 
        // can be assumed not to border zero (unless the paths are way too large)
        var heightmap = CalcPathHeightmapAtLocation(id, pos);
        if (math.abs(heightmap) > 0.1f)
            return false;

        bool seenPos = false;
        bool seenNeg = false;

        for (int i = 0; i < 4; i++)
        {
            var p = pos + pathSearchRadius * MathUtils.SignPairs[i];
            var h = CalcPathHeightmapAtLocation(id, p);

            if (h > 0) seenPos = true;
            else seenNeg = true;

            if (seenPos && seenNeg)
                return true;
        }

        return false;
    }

    public float CalcTerrainHeight(ChunkID id, float2 pos, bool normalize = false)
        => heightNoise.Get(id, pos, normalize);

    private void GenerateHeight(ChunkInstance chunk)
    {
        using var marker = ProfilerUtil.Enter("Generation.GenerateChunk.Height");

        foreach (var pos in Griderable.ForInclusive(gridCount))
        {
            ref var vertex = ref chunk.GroundMesher.Vertices[pos];

            // Apply height calculations //
            var vpos = vertex.tofloat3().xz;
            vpos += (float2)chunk.Position * gridSideLength;

            vertex.y = CalcTerrainHeight(chunk.ID, vpos);

            // Populate vertex colors based on vertex height //
            chunk.GroundMesher.Colors[pos] = groundColor.Evaluate(Mathf.InverseLerp(groundColorMinHeight, groundColorMaxHeight, vertex.y));
        }
    }

    private void GeneratePaths(ChunkInstance chunk)
    {
        using var marker = ProfilerUtil.Enter("Generation.GenerateChunk.Paths");

        foreach (var pos in Griderable.ForInclusive(gridCount))
        {
            ref var pathVertex = ref chunk.PathMesher.Vertices[pos];
            ref var grndVertex = ref chunk.GroundMesher.Vertices[pos];

            pathVertex.y = grndVertex.y;

            var vpos = pathVertex.tofloat3().xz;
            vpos += (float2)chunk.Position * gridSideLength;

            if (!CalcIsPath(chunk.ID, vpos))
            {
                pathVertex.y -= pathIndent;
            }
            else
            {
                grndVertex.y -= pathIndent;
            }
        }
    }

    private void GenerateProps(ChunkInstance chunk)
    {
        using var marker = ProfilerUtil.Enter("Generation.GenerateChunk.Props");

        // Place props //
        foreach (var placer in propPlacers)
            placer.PlaceInChunk(chunk);
    }

    /// <summary>
    /// Generate a new chunk at the given location.
    /// </summary>
    private async UniTask GenerateTask(int2 location)
    {
        // Get the instance of the chunk and set up its ID and location //
        var chunk = chunkPool.Get();
        generatingChunks.Add(location);

        await UniTask.SwitchToThreadPool();

        chunk.ID = ChunkID.Unique();
        chunk.Position = location;

        if (chunk.NeedsInitialization)
        {
            Array.Fill(chunk.PathMesher.Colors.Unraveled, pathColor);

            chunk.NeedsInitialization = false;
        }

        GenerateHeight(chunk);
        GeneratePaths(chunk);

        await UniTask.SwitchToMainThread();

        // Finalize the mesh and update the instance location //
        chunk.UpdateMeshInfo();
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

        // Cache this chunk //
        loadedChunks.Add(location, chunk);

        // Place props //
        GenerateProps(chunk);

        // End generation //
        generatingChunks.Remove(location);
    }

    private void Generate(int2 location)
    {
        GenerateTask(location).Forget();
    }

    void Awake()
    {
        // Setup the singleton //
        Instance = this;

        // Setup the pool for the chunks //
        chunkPool = InstancePool.OfPrefab(chunkPrefab, transform);

        forestDampenWeight = new(forestAffectGridSize, 1);
        ChunkCachers.Unregister(forestDampenWeight);
    }

    bool ChunkShouldCull(int2 chunk, int2 center)
        => math.csum(math.abs(chunk - center)) > GenerationRadiusInChunks;

    void Update()
    {
        var center = WorldToChunkPosition(generationCenter.position.tofloat3().xz);

        // Unload all chunks that are too far away //
        // This should happen first so new chunks can use those unloaded here //
        using (ProfilerUtil.Enter("Generation.UnloadOldChunks"))
        {
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
                ChunkCachers.UnloadChunk(instance.ID);
                chunkPool.Release(instance);
            }

            ListPool<int2>.Release(chunksToUnload);
        }

        // Generate new chunks as necessary //
        using (ProfilerUtil.Enter("Generation.GenerateChunks"))
        {
            foreach (var offset in Griderable.ForInclusive(-GenerationRadiusInChunks, GenerationRadiusInChunks))
            {
                var chunk = offset + center;

                if (ChunkShouldCull(chunk, center))
                    continue;

                if (!loadedChunks.ContainsKey(chunk) && !generatingChunks.Contains(chunk))
                    Generate(chunk);
            }
        }
    }
}
