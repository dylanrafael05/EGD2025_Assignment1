using Unity.Mathematics;

public class RockPlacer : ScenePropChanceFieldPlacer
{
    public float chance;

    public override float GetChance(ChunkID id, float2 pos)
        => GeneratorManager.Instance.CalcIsRocky(id, pos) ? chance : 0;
}
