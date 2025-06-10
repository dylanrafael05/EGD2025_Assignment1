using System;
using UnityEngine;

public class CampFireManager : MonoBehaviour
{
    public static CampFireManager instance;

    [NonSerialized] private CampFireVFXComponent campFireVFXComponent;
    [NonSerialized] private int fireStrength;



    void Awake()
    {
        campFireVFXComponent = GetComponent<CampFireVFXComponent>();
    }

    public void IncreaseBurn(int incrementValue)
    {
        fireStrength += incrementValue;
        campFireVFXComponent.SetLightIntensity(fireStrength);
    }
}
