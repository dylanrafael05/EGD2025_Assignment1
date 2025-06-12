using Unity.Mathematics;

public class GrassPlacer : ScenePropChanceFieldPlacer
{
    public float chance;

    public override float GetChance(ChunkID id, float2 pos)
        => GeneratorManager.Instance.CalcIsRocky(id, pos) ? 0 : chance;
}