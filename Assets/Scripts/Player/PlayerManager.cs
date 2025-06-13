using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum PlayerState
    {
        Idle = 0,
        Walking = 1,
        Running = 2
    }

    [NonSerialized] public static PlayerManager instance;
    [SerializeField] public PlayerState currentState = PlayerState.Idle;
    [SerializeField] public float rotation;

    [NonSerialized] private MovementComponent movementComponent;
    [NonSerialized] private PlayerCameraComponent cameraComponent;
    [NonSerialized] private PlayerAudioComponent audioComponent;
    [NonSerialized] private PlayerAnimationComponent playerAnimationComponent;
    [NonSerialized] private PlayerInteractionComponent playerInteractionComponent;
    [NonSerialized] private PlayerInventoryComponent playerInventoryComponent;
    [NonSerialized] private Incinerate incinerate;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        movementComponent = GetComponent<MovementComponent>();
        cameraComponent = GetComponent<PlayerCameraComponent>();
        audioComponent = GetComponent<PlayerAudioComponent>();
        playerAnimationComponent = GetComponent<PlayerAnimationComponent>();
        playerInteractionComponent = GetComponent<PlayerInteractionComponent>();
        playerInventoryComponent = GetComponent<PlayerInventoryComponent>();
        incinerate = GetComponent<Incinerate>();
    }



    void Update()
    {
        Vector2 currentMoveVector;
        if (playerInventoryComponent.TotalUniqueItem >= 1)
        {
            currentMoveVector = incinerate.Final();
        }
        else
        {
            currentMoveVector = InputManager.instance.MoveVector;
            rotation = cameraComponent.GenericPerspective(InputManager.instance.CameraFloat);
        }

        currentState = (PlayerState)movementComponent.GenericMove(currentMoveVector, rotation);
        audioComponent.GenericUpdate((int)currentState);
        playerAnimationComponent.GenericUpdate((int)currentState, currentMoveVector, rotation);
    }
}
