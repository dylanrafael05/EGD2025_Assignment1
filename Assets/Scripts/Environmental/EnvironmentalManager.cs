using System;
using UnityEngine;

public class EnvironmentalManager : MonoBehaviour
{
    public enum StormType
    {
        Clear = 0,
        Fog = 1,
        Rain = 2,
        Snow = 3
    }

    [NonSerialized] public static EnvironmentalManager instance;
    [NonSerialized] private StormManager stormManager;
    
    [Header("Storm System")]
    [SerializeField] public float stormStrength = 1;
    [SerializeField] public StormType currentType = StormType.Clear;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        stormManager = GetComponent<StormManager>();
    }



    void Update()
    {
        stormManager.UpdateStorm((int)currentType, stormStrength);
    }
}
