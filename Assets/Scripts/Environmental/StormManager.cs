using System;
using UnityEngine;

public class StormManager : MonoBehaviour
{
    [NonSerialized] private SnowComponent snowComponent;



    void Awake()
    {
        snowComponent = GetComponent<SnowComponent>();
    }



    public void UpdateStorm(int stormType, float stormStrength)
    {
        switch (stormType)
        {
            case 0:
                snowComponent.ClearStorm();
                break;
            case 1:

                break;
            case 2:
                
                break;
            case 3:
                snowComponent.SetStormStrength(stormStrength);
                break;
        }
    }
}
