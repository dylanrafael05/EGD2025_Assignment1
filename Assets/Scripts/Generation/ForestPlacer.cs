using Unity.Mathematics;
using UnityEngine;

public class ForestPlacer : ScenePropChanceFieldPlacer
{
    public override float GetChance(ChunkID id, float2 pos)
        => GeneratorManager.Instance.CalcForestChance(id, pos);
}
