using System;
using UnityEngine;

public class EnvironmentalManager : MonoBehaviour
{
    public enum StormType
    {
        Clear = 0,
        Fog = 1,
        Rain = 2, // currently does not exist
        Snow = 3
    }

    [NonSerialized] public static EnvironmentalManager instance;
    [NonSerialized] private StormManager stormManager;
    [NonSerialized] private FogManager fogManager;
    [NonSerialized] private StormAudioComponent stormAudioComponent;

    
    [Header("Storm System")]
    [SerializeField] public float stormStrength = 1;
    [SerializeField] public StormType currentType = StormType.Snow;


    [Header("Fog System")]
    [SerializeField] public float fogStrength = 1;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        stormManager = GetComponent<StormManager>();
        fogManager = GetComponent<FogManager>();
        stormAudioComponent = GetComponent<StormAudioComponent>();
    }



    void Update()
    {
        stormStrength = GeneratorManager.Instance.GetSnowDensity(transform.position.tofloat3().xz);
        fogStrength = GeneratorManager.Instance.GetFogDensity(transform.position.tofloat3().xz);

        stormManager.UpdateStorm((int)currentType, stormStrength);
        fogManager.UpdateFog(fogStrength);
        transform.position = PlayerManager.instance.transform.position;
        stormAudioComponent.AttemptPlayWind(stormStrength);
    }
}
