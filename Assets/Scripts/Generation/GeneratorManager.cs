using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Collections;
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

    [Header("Weather Patterns")]
    [SerializeField] private float weatherShiftSpeed;
    [SerializeField] private float fogNoiseScale;
    [SerializeField] private float snowNoiseScale;

    [Header("Generation Settings -- Noise")]
    [SerializeField] private CachedPerlinNoise.Fractal heightNoise;
    [SerializeField] private CachedPerlinNoise.Fractal forestNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathFirstNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathSecondNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathMixNoise;
    [SerializeField] private CachedPerlinNoise.Fractal pathSubtractNoise;

    [Header("Generation Settings -- Props")]
    [SerializeField] private ScenePropPlacer[] propPlacers;
    
    [Header("Generation Settings -- Start Area")]
    [SerializeField] private float startAreaInnerRadius;
    [SerializeField] private float startAreaOuterRadius;
    
    [Header("Generation Settings -- Void")]
    [SerializeField] private float voidRadius;
    [SerializeField] private float voidFalloff;
    [SerializeField] private float voidColorBeginHeight;
    [SerializeField] private float voidColorFullHeight;
    [SerializeField] private Color voidColor;
    
    [Header("Generation Settings -- Coloration")]
    [SerializeField] private float groundColorMinHeight;
    [SerializeField] private float groundColorMaxHeight;
    [SerializeField] private Color pathColor;
    [SerializeField] private Gradient groundColor;
    
    [Header("Generation Settings -- Forest")]
    [SerializeField] private float forestAffectGridSize = 3f;
    [SerializeField] private float forestDecayOnKill = 0.5f;
    
    [Header("Generation Settings -- Path")]
    [SerializeField] private float pathSearchRadius = 0.05f;
    [SerializeField] private float pathIndent = 0.1f;
    [SerializeField] private float pathSubEffect = 0.1f;
    [SerializeField] private float pathRadiusFromVoid = 3f;

    // Implementation variables //
    private InstancePool<ChunkInstance> chunkPool;

    private readonly HashSet<int2> generatingChunks = new();
    private readonly Dictionary<int2, ChunkInstance> loadedChunks = new();
    private CachedWeightMap forestDampenWeight;

    public float UnitSideLength => gridSideLength / gridCount;
    public int GenerationRadiusInChunks => Mathf.CeilToInt(generationRadius / gridSideLength);


    public float2 ClampInsideWorld(float2 position)
    {
        if (math.lengthsq(position) > voidRadius * voidRadius)
        {
            return math.normalize(position) * voidRadius;
        }

        return position;
    }

    public int2 WorldToChunkPosition(float2 position)
        => (int2)math.floor(position / gridSideLength);

    public ChunkInstance WorldToChunkInstance(float2 position)
        => loadedChunks.GetValueOrDefault(WorldToChunkPosition(position), null);
    
    public void KillForestAt(float2 pos)
        => Debug.Assert(forestDampenWeight.UpdateNearestIfExists(
            pos,
            (x, y) => Mathf.Clamp01(x * y),
            forestDecayOnKill));

    public float GetFogDensity(float2 pos)
    {
        pos *= fogNoiseScale;
        pos += 100;
        pos += Time.deltaTime * weatherShiftSpeed;

        return Mathf.PerlinNoise(pos.x, pos.y) * 100;
    }

    public float GetSnowDensity(float2 pos)
    {
        pos *= snowNoiseScale;
        pos -= 100;
        pos += Time.deltaTime * weatherShiftSpeed;

        return Mathf.PerlinNoise(pos.x, pos.y) * 10;
    }

    public float CalcForestDampen(ChunkID id, float2 pos)
        => forestDampenWeight.Get(id, pos);

    public float CalcForestChance(ChunkID id, float2 pos)
    {
        if (CalcDistanceIntoVoid(pos) > 0)
            return 0;

        var result = forestNoise.Get(id, pos, true)
                   * math.pow(1 - CalcTerrainHeight(id, pos, normalize: true), 0.5f)
                   * forestDampenWeight.Get(id, pos);

        return result * (1 - CalcCampfireOverrideFactor(pos));
    }

    public float CalcPathHeightmapAtLocation(ChunkID id, float2 pos)
    {
        var result = math.lerp(
            pathFirstNoise.Get(id, pos, true) * 2 - 1,
            pathSecondNoise.Get(id, pos, true) * 2 - 1,
            pathMixNoise.Get(id, pos, true)
        ) - pathSubtractNoise.Get(id, pos, true) * pathSubEffect;

        var start = CalcCampfireOverrideFactor(pos);

        result = math.lerp(result, start - 0.5f, start*start);
        result += math.pow(CalcDistanceIntoVoid(pos, -pathRadiusFromVoid), 3);

        return result;
    }

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

    public float CalcCampfireOverrideFactor(float2 pos)
    {
        if (math.lengthsq(pos) <= startAreaOuterRadius * startAreaOuterRadius)
        {
            var len = math.length(pos);
            var lerpToZero = 1 - math.smoothstep(startAreaInnerRadius, startAreaOuterRadius, len);

            return lerpToZero;
        }

        return 0;
    }

    public float CalcDistanceIntoVoid(float2 pos, float radiusOffset = 0)
    {
        var radius = voidRadius + radiusOffset;

        if (math.lengthsq(pos) >= radius * radius)
        {
            return math.length(pos) - radius;
        }

        return 0;
    }

    public float CalcVoidHeightOffset(ChunkID id, float2 pos)
        => -math.pow(
            CalcDistanceIntoVoid(pos, -voidFalloff / 2) / voidFalloff, 3);

    public float CalcTerrainHeight(ChunkID id, float2 pos, bool normalize = false)
        => math.lerp(
            heightNoise.Get(id, pos, normalize),
            heightNoise.TotalAmplitude / 2,
            CalcCampfireOverrideFactor(pos)
        );

    private void GenerateShape(ChunkInstance chunk)
    {
        using var marker = ProfilerUtil.Enter("Generation.GenerateChunk.Shape");

        foreach (var pos in Griderable.ForInclusive(gridCount))
        {
            ref var vertex = ref chunk.GroundMesher.Vertices[pos];

            // Apply height calculations //
            var vpos = vertex.tofloat3().xz;
            vpos += (float2)chunk.Position * gridSideLength;

            var voidOffset = CalcVoidHeightOffset(chunk.ID, vpos);

            vertex.y = CalcTerrainHeight(chunk.ID, vpos) + voidOffset;

            // Populate vertex colors based on vertex height //
            chunk.GroundMesher.Colors[pos] = Color.Lerp(
                groundColor.Evaluate(Mathf.InverseLerp(groundColorMinHeight, groundColorMaxHeight, vertex.y)),
                voidColor,
                1 - Mathf.Clamp01(
                    Mathf.InverseLerp(voidColorFullHeight, voidColorBeginHeight, voidOffset))
            );
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

        GenerateShape(chunk);
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
