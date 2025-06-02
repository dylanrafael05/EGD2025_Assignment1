using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [NonSerialized] public static AudioManager instance;

    [Header("Audio System")]
    [SerializeField] private int sourceCount = 1;


    [Header("Audio Reference")]
    [SerializeField] private List<AudioSource> sourceCache;
    [SerializeField] private List<Audio> initiationReference;
    [NonSerialized] private Dictionary<string, Audio> audioReference;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        audioReference = new Dictionary<string, Audio>();

        for (int i = 1; i < sourceCount; i++)
        {
            sourceCache.Add(Instantiate(sourceCache[0], transform));
        }

        foreach (Audio entry in initiationReference)
        {
            audioReference.Add(entry.name, entry);
        }
    }
    void Start()
    {
        AudioManager.instance.PlayGeneric("Test");
    }


    private int FindFreeSource()
    {
        for (int i = 0; i < sourceCache.Count; i++)
        {
            if (!sourceCache[i].isPlaying)
            {
                return i;
            }
        }
        return -1;
    }

    public int PlayGeneric(String audioName)
    {
        if (!audioReference.ContainsKey(audioName))
        {
            return -1;
        }

        int ad = FindFreeSource(); // Audio Descriptor

        if (ad == -1)
        {
            return -1;
        }

        AudioSource activateAudioSource = sourceCache[ad];
        activateAudioSource.loop = false;
        activateAudioSource.volume = audioReference[audioName].volume;
        activateAudioSource.clip = audioReference[audioName].clip;
        activateAudioSource.Play();

        return ad;
    }

    public int PlayLoop(String audioName)
    {
        int ad = PlayGeneric(audioName);
        
        if (ad == -1)
        {
            return -1;
        }

        sourceCache[ad].loop = true;
        return ad;
    }

    public int StopGeneric(int ad)
    {
        if (!sourceCache[ad].isPlaying)
        {
            return -1;
        }
        sourceCache[ad].Stop();
        return -1;
    }    
}
