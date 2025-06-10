using UnityEngine;

public class SpriteComponent : MonoBehaviour
{
    [SerializeField] GameObject spriteSocket;



    void Update()
    {
        spriteSocket.transform.forward = -PlayerCameraComponent.instance.cameraComponent.transform.forward;
    }
}
