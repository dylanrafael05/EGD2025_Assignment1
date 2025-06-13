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

        fireEffect.SetEmission(lightValue + 5f);
        var main = fireEffect.main;
        main.startSize = new(1, math.pow(lightValue, 0.7f) + 1f);
        fireEffect.SetHeading(new Vector3(1,0,0) * Mathf.Pow(GeneratorManager.Instance.GetSnowDensity(transform.position.tofloat3().xz), 1.4f));
    }
}
