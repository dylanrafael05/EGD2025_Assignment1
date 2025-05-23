using UnityEngine;

public class GeneratorSettings : MonoBehaviour
{
    public static GeneratorSettings Instance { get; private set; }

    [Header("Settings")]
    public int gridCount;
    public float unitSideLength;

    void Awake()
    {
        Instance = this;
    }
}
