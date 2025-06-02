using System;
using UnityEngine;

public class PlayerCameraComponent : MonoBehaviour
{
    [SerializeField] private GameObject cameraSocket;
    [SerializeField] private Camera cameraComponent;
    [SerializeField] private float rotationLerpFactor;
    [NonSerialized] private float desiredRotation = 45;



    public float GenericPerspective(float direction)
    {
        cameraSocket.transform.localRotation = Quaternion.Lerp(cameraSocket.transform.localRotation, Quaternion.Euler(45, desiredRotation, transform.eulerAngles.z), rotationLerpFactor * Time.deltaTime);

        if (direction > 0)
        {
            desiredRotation += 90;
        }
        else if (direction < 0)
        {
            desiredRotation -= 90;
        }
        return desiredRotation;
    } 
}
