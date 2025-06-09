using System;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{
    [NonSerialized] private BoxCollider boxCollider;



    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == Layers.InteractableMask)
        {
            return;
        }

        if (other.gameObject.layer == 8)
        {
            if (other.CompareTag("Tree") && InputManager.instance.interactBool)
            {
                print("hallo");
            }
            else if (false)
            {

            }
        }
    }
}
