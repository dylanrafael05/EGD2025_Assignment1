using System;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PostProcessEffects : MonoBehaviour
{
    public static PostProcessEffects Instance { get; private set; }

    public async UniTask CollectItem()
    {
        GeneratorManager.Instance.SignalWillRegenerate();
        InspectDisplay.Instance.ForceDisplay = true;

        target = 1;
        faderImage.CrossFadeAlpha(1, 1f, false);
        await UniTask.WaitForSeconds(1.0f);

        GeneratorManager.Instance.Regenerate();
        await UniTask.WaitForSeconds(0.5f);

        target = 0;
        faderImage.CrossFadeAlpha(0, 0.5f, true);
        await UniTask.WaitForSeconds(1.0f);

        InspectDisplay.Instance.ForceDisplay = false;
    }

    [SerializeField] private Image faderImage;
    [SerializeField] private VolumeProfile volume;
    [SerializeField] private float vignetteLerpSpeed;

    private Vignette vignette;

    private float target;
    private float baseVignette;

    public void SetTarget(float target)
    {
        this.target = target;
    }

    void Awake()
    {
        Instance = this;

        volume.TryGet(out vignette);
        baseVignette = vignette.intensity.value;

        faderImage.enabled = true;
        faderImage.CrossFadeAlpha(1, 0, false);
        faderImage.CrossFadeAlpha(0, 4.0f, false);
    }

    void Update()
    {
        vignette.intensity.value = MathUtils.EaseTowards(
            vignette.intensity.value, math.lerp(baseVignette, 1, target),
            vignetteLerpSpeed, Time.deltaTime);
    }

}
