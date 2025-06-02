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
    [NonSerialized] public Vector3 position;
    [NonSerialized] public float rotation;

    [NonSerialized] private MovementComponent movementComponent;
    [NonSerialized] private PlayerCameraComponent cameraComponent;
    [NonSerialized] private PlayerAudioComponent audioComponent;



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
    }



    void Update()
    {
        rotation = cameraComponent.GenericPerspective(InputManager.instance.cameraFloat);
        currentState = (PlayerState)movementComponent.GenericMove(InputManager.instance.moveVector, rotation);
        audioComponent.UpdateGeneric((int)currentState);
    }
}
