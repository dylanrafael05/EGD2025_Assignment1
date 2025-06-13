using Unity.Mathematics;

public class CabinCompass : CompassBase
{
    public override float2 GetTarget()
        => GeneratorManager.Instance.CabinLocation;
}
