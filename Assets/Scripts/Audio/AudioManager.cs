using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [NonSerialized] public static AudioManager instance;

    [Header("Audio System")]
    [SerializeField] int sourceCount = 1;


    [Header("Audio Reference")]
    [SerializeField] public List<AudioSource> sourceCache;
    [SerializeField] List<Audio> initiationReference;
    [NonSerialized] public Dictionary<string, Audio> audioReference;



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



    public int PlayGeneric(String audioName)
    {
        if (!audioReference.ContainsKey(audioName))
        {
            return -1;
        }

        AudioSource activateAudioSource = sourceCache[FindFreeSource()];
        activateAudioSource.volume = audioReference[audioName].volume;
        activateAudioSource.clip = audioReference[audioName].clip;
        activateAudioSource.Play();

        return 0;
    }

    public int FindFreeSource()
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
}
