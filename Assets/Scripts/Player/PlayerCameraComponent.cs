using System;
using UnityEngine;

public class PlayerCameraComponent : MonoBehaviour
{
    public static PlayerCameraComponent instance;
    [SerializeField] private GameObject cameraSocket;
    [SerializeField] public Camera cameraComponent;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float rotationLerpFactor;
    [SerializeField] private float xAngle;

    [NonSerialized] private float desiredRotation = 45;



    void Awake()
    {
        instance = this;
    }



    public float GenericPerspective(float direction)
    {
        cameraSocket.transform.localRotation = Quaternion.Lerp(
            cameraSocket.transform.localRotation,
            Quaternion.Euler(xAngle, desiredRotation, 0),
            rotationLerpFactor * Time.deltaTime
        );
        spriteRenderer.transform.forward = -cameraComponent.transform.forward;

        if (direction > 0)
        {
            desiredRotation -= 90;
        }
        else if (direction < 0)
        {
            desiredRotation += 90;
        }
        return desiredRotation;
    } 
}
