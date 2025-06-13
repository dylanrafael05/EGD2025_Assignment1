using Unity.Mathematics;
using UnityEngine;

public abstract class ScenePropChanceFieldPlacer : ScenePropPlacer
{
    [SerializeField] private float maxChance;
    [SerializeField] private float sampleSize;
    [SerializeField] private float chancePower = 1;

    public abstract float GetChance(ChunkID id, float2 pos);

    public override void PlaceInChunk(ChunkInstance chunk)
    {
        var sampleCount = Mathf.RoundToInt(chunk.Bounds.width / sampleSize);
        var sampleStep = chunk.Bounds.width / (sampleCount + 1);

        foreach (var offset in Griderable.For(sampleCount))
        {
            var pos = chunk.Bounds.min.tofloat2() + (math.float2(offset) + 0.5f) * sampleStep;
            var nudge = UnityEngine.Random.insideUnitCircle * sampleStep / 2;

            if (math.lengthsq(pos) > math.square(GeneratorManager.Instance.VoidRadius))
                continue;

            pos += (float2)nudge;
            var density = GetChance(chunk.ID, pos);

            if (UnityEngine.Random.value < math.pow(density, chancePower) * maxChance)
            {
                AttemptCreate(chunk, pos);
            }
        }
    }
}