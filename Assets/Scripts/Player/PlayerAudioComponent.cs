using System;
using System.Collections;
using UnityEngine;

public class PlayerAudioComponent : MonoBehaviour
{
    [NonSerialized] private int walkingAD = -1;



    public void UpdateGeneric(int state)
    {
        switch (state)
        {
            case 0:
                if (walkingAD == -1)
                {
                    break;
                }
                walkingAD = AudioManager.instance.StopGeneric(walkingAD);
                break;
            case 1:
                if (walkingAD != -1)
                {
                    break;
                }
                walkingAD = AudioManager.instance.PlayLoop("Walking");
                break;
        }
    }
}
