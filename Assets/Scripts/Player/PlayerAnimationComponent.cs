using UnityEngine;

public class PlayerAnimationComponent : MonoBehaviour
{
    [SerializeField] Animator animator;
    Vector2 currentDirection;
    float lastRotation;



    void Awake()
    {
        animator = GetComponent<Animator>();
        currentDirection = new Vector2();
    }

    public int GenericUpdate(int playerState, Vector2 moveVector, float rotation)
    {
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
                    return 0;
                }
            case 1:
                {
                    currentDirection = moveVector;
                    lastRotation = rotation;
                    // moveVector (x, y) : x, y either -1, 0 , 1 : hook up animation
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
