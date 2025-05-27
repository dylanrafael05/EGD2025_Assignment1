using System.Collections;
using UnityEngine;

public class SceneProp : PoolableBehaviour
{
    [Header("Prop Settings")]
    [SerializeField] private BoxCollider bounds;
    [SerializeField] private bool randomizeRotation;
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;

    public BoxCollider Bounds => bounds;

    public void ApplyRandomization()
    {
        if (randomizeRotation)
        {
            transform.localEulerAngles = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);
        }

        var scale = UnityEngine.Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
