using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimationComponent : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sprite;
    Vector2 currentDirection;
    float lastRotation;



    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        currentDirection = new Vector2();
    }

    public int GenericUpdate(int playerState, Vector2 moveVector, float rotation)
    {
        var angle = Vector2.SignedAngle(moveVector, Vector2.down);
        animator.SetFloat("lookAngleAbs", math.abs(angle / 180));
        sprite.flipX = angle < 0 && math.abs(angle) < 170;

        switch (playerState)
        {
            case 0:
                {
                    if (lastRotation != rotation)
                    {
                        float rotationDifference = rotation - lastRotation;
                        lastRotation = rotation;
                        // switch sprite to the correct view (camera is looking at bro at a new angle) based on rotationDifference, use this to determine idle state

                        return 0;
                    }

                    // use currentDirection to determine final idle state
                    animator.SetBool("isMoving", false);
                    return 0;
                }
            case 1:
                {
                    currentDirection = moveVector;
                    lastRotation = rotation;
                    // moveVector (x, y) : x, y either -1, 0 , 1 : hook up animation
                    animator.SetBool("isMoving", true);
                    return 1;
                }
            case 2:
                {

                    return 2;
                }
        }
        return -1;
    }
}
