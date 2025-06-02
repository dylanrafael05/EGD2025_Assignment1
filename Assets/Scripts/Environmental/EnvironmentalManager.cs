using System;
using UnityEngine;

public class EnvironmentalManager : MonoBehaviour
{
    public enum WeatherType
    {
        Clear = 0,
        Fog = 1,
        Rain = 2,
        Snow = 3
    }

    [NonSerialized] public static EnvironmentalManager instance;
    [SerializeField] public int stormStrength = 1;
    [SerializeField] public WeatherType currentType = WeatherType.Clear;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }



    void Update()
    {
        
    }
}
