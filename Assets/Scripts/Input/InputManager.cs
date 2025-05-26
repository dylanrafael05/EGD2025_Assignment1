using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [NonSerialized] public static InputManager instance; 
    private InputAction moveAction;
    [NonSerialized] public Vector2 moveVector; // [i, j] floats



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
        moveAction = InputSystem.actions.FindAction("Move");
    }



    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
    }
}
