using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [NonSerialized] public static InputManager instance; 
    [NonSerialized] private InputAction moveAction;
    [NonSerialized] public Vector2 moveVector; // [x, z] floats
    [NonSerialized] private InputAction cameraAction;
    [NonSerialized] public float cameraFloat;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        moveAction = InputSystem.actions.FindAction("Move");
        cameraAction = InputSystem.actions.FindAction("Camera");
    }



    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
        if (cameraAction.WasPressedThisFrame())
        {
            cameraFloat = cameraAction.ReadValue<float>();
        }
        else
        {
            cameraFloat = 0;
        }
    }
}
