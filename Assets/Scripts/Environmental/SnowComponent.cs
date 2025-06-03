using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SnowComponent : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> snowVFX;

    [Header("SnowVFXDefault")]
    [SerializeField] private float defaultEmmision;
    [SerializeField] private Vector2 defaultStartLifeTime;
    [SerializeField] private Vector2 defaultSpeed;
    [SerializeField] private Vector2 defaultDirection;



    void Awake()
    {
        defaultEmmision = snowVFX[0].GetEmission();
        defaultStartLifeTime = snowVFX[0].GetStartLifeTime();
        defaultSpeed = snowVFX[0].GetStartSpeed();
        defaultDirection = snowVFX[0].GetDirection();
    }



    public void SetStormStrength(float stormStrength)
    {
        snowVFX[0].SetEmission(defaultEmmision + Mathf.Abs(defaultEmmision * stormStrength / 4)); snowVFX[1].SetEmission(defaultEmmision + Mathf.Abs(defaultEmmision * stormStrength / 4));
        snowVFX[0].SetStartLifeTime(defaultStartLifeTime * (1/stormStrength)); snowVFX[1].SetStartLifeTime(defaultStartLifeTime * (1/stormStrength));
        snowVFX[0].SetStartSpeed(defaultSpeed * (stormStrength)); snowVFX[1].SetStartSpeed(defaultSpeed * (stormStrength));
        snowVFX[0].SetDirection(defaultDirection * (stormStrength)); snowVFX[1].SetDirection(defaultDirection * (stormStrength));
    }

    public void ClearStorm()
    {
         snowVFX[0].SetEmission(0); snowVFX[1].SetEmission(0);
    }
}
