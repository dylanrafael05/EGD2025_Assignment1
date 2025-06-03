using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ForestPlacer : ScenePropPlacer
{
    [SerializeField] private float maxChance;
    [SerializeField] private float sampleSize;
    [SerializeField] private float chancePower = 1;

    public override void PlaceInChunk(ChunkInstance chunk)
    {
        var sampleCount = Mathf.RoundToInt(chunk.Bounds.width / sampleSize);
        var sampleStep = chunk.Bounds.width / (sampleCount + 1);

        foreach (var offset in Griderable.For(sampleCount))
        {
            var pos = chunk.Bounds.min.tofloat2() + (math.float2(offset) + 0.5f) * sampleStep;
            var nudge = UnityEngine.Random.insideUnitCircle * sampleStep / 2;

            pos += (float2)nudge;
            var density = GeneratorManager.Instance.CalcForestChance(chunk.ID, pos);

            if (UnityEngine.Random.value < math.pow(density, chancePower) * maxChance)
            {
                AttemptCreate(chunk, pos);
            }
        }
    }
}