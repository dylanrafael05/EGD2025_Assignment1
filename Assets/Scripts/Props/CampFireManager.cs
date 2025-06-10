using System;
using UnityEngine;

public class CampFireManager : MonoBehaviour
{
    public static CampFireManager instance;

    [NonSerialized] private CampFireVFXComponent campFireVFXComponent;
    [NonSerialized] private int fireStrength;

    public int TotalUsedLogs => fireStrength;

    void Awake()
    {
        campFireVFXComponent = GetComponent<CampFireVFXComponent>();
        instance = this;
    }

    public void IncreaseBurn(int incrementValue)
    {
        fireStrength += incrementValue;
        campFireVFXComponent.SetLightIntensity(fireStrength);
    }
}
