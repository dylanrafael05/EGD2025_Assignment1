using System.Collections.Generic;
using UnityEngine;

public class FogComponent : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> fogVFX;

    [Header("FogVFXDefault")]
    [SerializeField] private float defaultEmmision;
    [SerializeField] private Vector2 defaultStartLifeTime;


    void Awake()
    {
        defaultEmmision = fogVFX[0].GetEmission();
        //defaultStartLifeTime = fogVFX[0].GetStartLifeTime();
    }

    public void SetStormStrength(float fogStrength)
    {
        fogVFX[0].SetEmission(fogStrength);
        //fogVFX[0].SetStartLifeTime(defaultStartLifeTime+new Vector2(fogStrength, fogStrength));
    }
}
