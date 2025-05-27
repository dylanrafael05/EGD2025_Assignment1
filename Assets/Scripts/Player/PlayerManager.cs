using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public static PlayerManager instance;
    [NonSerialized] public Vector3 position;
    [NonSerialized] public float rotation;

    private PlayerMovementComponent playerMovementComponent;
    private CameraComponent cameraComponent;


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
        playerMovementComponent = GetComponent<PlayerMovementComponent>();
        cameraComponent = GetComponent<CameraComponent>();
    }



    void Update()
    {
        rotation = cameraComponent.GenericPerspective(InputManager.instance.cameraFloat);
        playerMovementComponent.GenericMove(InputManager.instance.moveVector, rotation);
    }
}
