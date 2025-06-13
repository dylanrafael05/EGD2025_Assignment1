using UnityEngine;

public class StormAudioComponent : MonoBehaviour
{
    public int AttemptPlayWind(float stormStrength)
    {
        if (UnityEngine.Random.Range(0f, 1f) < 0.02f * (stormStrength/10))
        {
            return AudioManager.instance.PlayRandomPitch("Wind", transform.position + new Vector3(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f)));
        }
        return -1;
    }
}
