using System;
using UnityEngine;

[System.Serializable]
public class Audio
{
    [SerializeField] public string name = "default";
    [SerializeField] public AudioClip clip = null;
    [SerializeField] public float volume = 1;
}
