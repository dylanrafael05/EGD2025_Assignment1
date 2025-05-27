using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 1;
    private Rigidbody rb;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    public bool GenericMove(Vector2 moveVector, float rotation)
    {
        float3 tempVector = new float3(moveVector.x * playerSpeed * Time.deltaTime,
                                        rb.linearVelocity.y,
                                        moveVector.y * playerSpeed * Time.deltaTime);
        rb.linearVelocity = Quaternion.Euler(0, rotation, 0) * tempVector;;
        
        //SnapToGround();
        if (moveVector.x == 0 && moveVector.y == 0)
        {
            return false;
        }
        return true;
    }

    private float SnapToGround()
    {
        float y = GenerationUtils.GroundHeightAt(transform.position.tofloat3().xz);
        if (float.IsNaN(y)) {
            return 0.0f;
        }
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        return y;
    }
}
