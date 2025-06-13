using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 10;
    [NonSerialized] private Rigidbody rb;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    public int GenericMove(Vector2 moveVector, float rotation)
    {
        float3 tempVector = new float3(moveVector.x * moveSpeed,
                                        rb.linearVelocity.y,
                                        moveVector.y * moveSpeed);
        rb.linearVelocity = Quaternion.Euler(0, rotation, 0) * tempVector;;
        
        var loc = transform.position.tofloat3();
        loc.y = GenerationUtils.GroundHeightAt(loc.xz);
        loc.xz = GeneratorManager.Instance.ClampInsideWorld(loc.xz);
        transform.position = loc;
        
        if (moveVector.x == 0 && moveVector.y == 0)
        {
            return 0;
        }
        return 1;
    }

    private void SnapToGround()
    {
        var loc = transform.position.tofloat3();
        loc.y = GenerationUtils.StandHeightAt(loc.xz);
        loc.xz = GeneratorManager.Instance.ClampInsideWorld(loc.xz);
        transform.position = loc;
    }
}
