using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CampFireVFXComponent : MonoBehaviour
{
    [SerializeField] private Light pointLight;
    [NonSerialized] private float flickerValue = 0;
    [SerializeField] private ParticleSystem fireEffect;



    void Awake()
    {
        pointLight = GetComponentInChildren<Light>();
        fireEffect = GetComponentInChildren<ParticleSystem>();
    }



    public void SetLightIntensity(float lightValue)
    {
        pointLight.intensity = lightValue + Mathf.Sin(flickerValue) * Random.Range(0.9f, 1.1f);
        flickerValue += Time.deltaTime * 0.01f;

        fireEffect.SetEmission(math.min(lightValue, 0) + 1f);
        var main = fireEffect.main;
        main.startSize = new(1, math.pow(lightValue, 0.7f) + 1f);
    }
}
