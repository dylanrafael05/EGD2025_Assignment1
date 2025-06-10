using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [NonSerialized] public static InputManager instance; 

    [NonSerialized] private InputAction moveAction;
    public Vector2 MoveVector => moveAction.ReadValue<Vector2>(); // [x, z] floats

    [NonSerialized] private InputAction cameraAction;
    public float CameraFloat => cameraAction.WasPressedThisFrame()
        ? cameraAction.ReadValue<float>() 
        : 0;

    [NonSerialized] private InputAction interactAction;
    private bool interactBool = false;
    public bool InteractBool
    {
        get
        {
            if (interactAction.WasPressedThisFrame())
                interactBool = true;

            return interactBool;
        }
        set
        {
            interactBool = value;
        }
    }
    
    [NonSerialized] private InputAction inspectAction;
    public bool InspectBool => inspectAction.IsPressed();

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
        interactAction = InputSystem.actions.FindAction("Interact");
        inspectAction = InputSystem.actions.FindAction("Inspect");
    }
}
