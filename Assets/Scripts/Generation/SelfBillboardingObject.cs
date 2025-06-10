using UnityEngine;

/// <summary>
/// A simple helper behaviour to billboard an object to face the camera.
/// </summary>
public class SelfBillboardingObject : MonoBehaviour
{
    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
