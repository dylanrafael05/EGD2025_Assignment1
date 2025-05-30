using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [NonSerialized] public AudioManager instance;

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
    }
    void Start()
    {
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



    public int PlayAudioGeneric()
    {
        return 0;
    }
}
