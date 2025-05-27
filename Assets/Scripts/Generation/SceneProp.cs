using UnityEngine;

public class SceneProp : PoolableBehaviour
{
    [Header("Prop Settings")]
    [SerializeField] private BoxCollider bounds;

    public BoxCollider Bounds => bounds;

    public void SnapToGround()
    {
        var pos = transform.position;
        pos.y = GenerationUtils.GroundHeightAt(pos.asfloat3().xz);
        
        transform.position = pos;
    }
}
