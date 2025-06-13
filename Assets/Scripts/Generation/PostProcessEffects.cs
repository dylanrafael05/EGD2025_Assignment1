using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessEffects : MonoBehaviour
{
    [SerializeField] private VolumeProfile volume;
    private Vignette vignette;

    void Awake()
    {
        volume.TryGet(out vignette);
    }
}
