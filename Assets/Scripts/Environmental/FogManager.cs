using System;
using UnityEngine;

public class FogManager : MonoBehaviour
{
    [NonSerialized] private FogComponent fogComponent;

    void Awake()
    {
        fogComponent = GetComponent<FogComponent>();
    }



    public void UpdateFog(float fogStrength)
    {
        fogComponent.SetStormStrength(fogStrength);
    }
}
