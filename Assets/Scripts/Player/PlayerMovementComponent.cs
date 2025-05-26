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



    public bool GenericMove(Vector2 moveVector)
    {
        rb.linearVelocity = new Vector3(moveVector.x*playerSpeed, rb.linearVelocity.y, moveVector.y*playerSpeed);
        if (moveVector.x == 0 && moveVector.y == 0)
        {
            return false;
        }
        return true;
    }
}
