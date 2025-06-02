using System;
using System.Collections.Generic;
using UnityEngine;

public class SnowComponent : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> snowVFX;

    [Header("SnowVFXDefault")]
    [SerializeField] private float defaultEmmision;
    [SerializeField] private float[] defaultVel;
    [SerializeField] private Vector2 defaultDirection;



    void Awake()
    {
        defaultVel = new float[2];

        defaultEmmision = snowVFX[0].emission.rateOverTime.constant;
        defaultVel[0] = snowVFX[0].main.startSpeed.constantMin; defaultVel[1] = snowVFX[0].main.startSpeed.constantMax;
    }



    void Start()
    {
        
    }
}
