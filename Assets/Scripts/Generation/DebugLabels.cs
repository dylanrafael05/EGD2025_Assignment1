using System;
using UnityEngine;

public class DebugLabels : MonoBehaviour
{
    public static DebugLabels Instance { get; private set; }

    [SerializeField]
    private DebugLabel prefab;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public static void Attach<T>(GameObject follow, Vector3 offset, Func<T> text, Func<bool> shouldShow = null)
    {
        if (Instance == null)
            return;
        
        shouldShow ??= () => true;
        
        var instance = Instantiate(Instance.prefab, Instance.transform);
        instance.name = $"{follow.name} -- [[LABEL]]";
        instance.Setup(follow, offset, () => text().ToString(), shouldShow);
    }
}
