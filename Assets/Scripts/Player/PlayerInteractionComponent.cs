using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{
    [NonSerialized] private BoxCollider boxCollider;
    [NonSerialized] private PlayerInventoryComponent playerInventoryComponent;



    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        playerInventoryComponent = GetComponent<PlayerInventoryComponent>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == Layers.InteractableMask)
        {
            return;
        }

        if (other.gameObject.layer != 8)
        {
            return;
        }
                    
        if (other.CompareTag("Tree"))
        {
            if (InputManager.instance.interactBool)
            {
                playerInventoryComponent.ObtainFireWood();
                other.gameObject.transform.parent.parent.gameObject.GetComponent<TreeProp>().ChopDown();
            }
        }
        else if (other.CompareTag("Item"))
        {
            if (InputManager.instance.interactBool)
            {
                playerInventoryComponent.ReplaceItem(other.gameObject);
            }
        }
        else if (other.CompareTag("CampFire"))
        {
            if (InputManager.instance.interactBool)
            {
                other.GetComponent<CampFireManager>().IncreaseBurn(playerInventoryComponent.BurnInventory());
            }
        }
    }
}
