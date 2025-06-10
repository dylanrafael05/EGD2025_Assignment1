using Unity.Mathematics;
using UnityEngine;

public class CampfirePlacer : ScenePropPlacer
{
    public override void PlaceInChunk(ChunkInstance chunk)
    {
        if (chunk.Bounds.Contains(Vector2.zero))
        {
            AttemptCreate(chunk, float2.zero, false);
        }
    }
}
