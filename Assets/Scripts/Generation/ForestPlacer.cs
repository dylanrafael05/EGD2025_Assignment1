using Unity.Mathematics;
using UnityEngine;

public class ForestPlacer : ScenePropPlacer
{
    [SerializeField] private float densityMapScale;
    [SerializeField] private float maxChance;
    [SerializeField] private float sampleSize;

    public override void PlaceInChunk(ChunkInstance chunk)
    {
        var sampleCount = Mathf.RoundToInt(chunk.Bounds.width / sampleSize);
        var sampleStep = sampleCount * sampleSize;

        foreach (var off in Griderable.For(sampleCount))
        {
            var offset = off - sampleCount / 2;

            var pos = chunk.transform.position.tofloat3().xz + math.float2(offset) * sampleStep;

            var density = GenerationUtils.Noise.Octave(densityMapScale).Get(chunk.ID, pos);

            if (UnityEngine.Random.value < density * maxChance)
            {
                var nudge = UnityEngine.Random.insideUnitCircle * sampleStep;

                AttemptCreate(chunk, pos + (float2)nudge);
            }
        }
    }
}