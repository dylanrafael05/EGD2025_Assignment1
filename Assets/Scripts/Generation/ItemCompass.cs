using Unity.Mathematics;
using UnityEngine;

public class ItemCompass : CompassBase
{
    public float timeScale;
    public override float2 GetTarget()
    {
        if (SpecialItemPlacer.Instance.ItemPlace is float2 location)
        {
            return location;
        }
        else
        {
            return math.float2(
                Mathf.PerlinNoise1D(Time.time * timeScale) * 2 - 1,
                Mathf.PerlinNoise1D(Time.time * timeScale + 1000) * 2 - 1
            ) * GeneratorManager.Instance.VoidRadius;
        }
    }
}
