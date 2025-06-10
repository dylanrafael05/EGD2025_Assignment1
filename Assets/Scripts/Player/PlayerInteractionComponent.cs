using System;
using System.Collections.Generic;
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

        if (InputManager.instance.InteractBool)
        {
            if (other.CompareTag("Tree"))
            {
                playerInventoryComponent.ObtainFireWood();
                other.gameObject.transform.parent.parent.gameObject.GetComponent<TreeProp>().ChopDown();
            }
            else if (other.CompareTag("Item"))
            {
                playerInventoryComponent.ReplaceItem(other.gameObject.GetComponent<ItemComponent>());
            }
            else if (other.CompareTag("CampFire"))
            {
                other.GetComponent<CampFireManager>().IncreaseBurn(playerInventoryComponent.BurnInventory());
            }

            InputManager.instance.InteractBool = false;
        }
    }
}
