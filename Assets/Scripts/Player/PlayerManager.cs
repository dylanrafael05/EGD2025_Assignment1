using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public static PlayerManager instance;
    [NonSerialized] public Vector3 position;

    private PlayerMovementComponent playerMovementComponent;


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
    }



    void Update()
    {
        playerMovementComponent.GenericMove(InputManager.instance.moveVector);
    }
}
