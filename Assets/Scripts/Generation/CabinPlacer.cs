public class CabinPlacer : ScenePropPlacer
{
    public override void PlaceInChunk(ChunkInstance chunk)
    {
        if (chunk.Bounds.Contains(GeneratorManager.Instance.CabinLocation))
        {
            AttemptCreate(chunk, GeneratorManager.Instance.CabinLocation, false);
        }
    }
}