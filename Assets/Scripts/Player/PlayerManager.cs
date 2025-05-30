using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public static PlayerManager instance;
    [NonSerialized] public Vector3 position;
    [NonSerialized] public float rotation;

    [NonSerialized] private PlayerMovementComponent movementComponent;
    [NonSerialized] private CameraComponent cameraComponent;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    void Start()
    {
        movementComponent = GetComponent<PlayerMovementComponent>();
        cameraComponent = GetComponent<CameraComponent>();
    }



    void Update()
    {
        rotation = cameraComponent.GenericPerspective(InputManager.instance.cameraFloat);
        movementComponent.GenericMove(InputManager.instance.moveVector, rotation);
    }
}
