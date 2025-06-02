using UnityEngine;

public static class ParticleStormVFX
{
    public static float GetEmission(this ParticleSystem stormVFX)
    {
        return stormVFX.emission.rateOverTime.constant;
    }

    public static void SetEmission(this ParticleSystem stormVFX, float rateOverTime)
    {
        ParticleSystem.EmissionModule emmision = stormVFX.emission;
        emmision.rateOverTime = rateOverTime;
    }

    public static Vector2 GetStartSpeed(this ParticleSystem stormVFX)
    {
        return new Vector2(stormVFX.main.startSpeed.constantMin, stormVFX.main.startSpeed.constantMax);
    }

    public static void SetStartSpeed(this ParticleSystem stormVFX, Vector2 startSpeed)
    {
        ParticleSystem.MainModule main = stormVFX.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(startSpeed.x, startSpeed.y);
    }

    public static Vector2 GetDirection(this ParticleSystem stormVFX)
    {
        return new Vector2(stormVFX.velocityOverLifetime.x.constant, stormVFX.velocityOverLifetime.z.constant);
    }

    public static void SetDirection(this ParticleSystem stormVFX, Vector2 direction)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = stormVFX.velocityOverLifetime;
        velocityOverLifetimeModule.x = direction.x;
        velocityOverLifetimeModule.z = direction.y;
    }
}
