using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CampFireVFXComponent : MonoBehaviour
{
    [SerializeField] private Light pointLight;
    [NonSerialized] private float flickerValue = 0;



    void Awake()
    {
        pointLight = GetComponentInChildren<Light>();
    }



    public void SetLightIntensity(float lightValue)
    {
        pointLight.intensity = lightValue + Mathf.Sin(flickerValue) * Random.Range(0.9f, 1.1f);
        flickerValue += Time.deltaTime*0.01f;
    }
}
