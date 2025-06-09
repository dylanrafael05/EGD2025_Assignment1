using UnityEngine;

public class PlayerInventoryComponent : MonoBehaviour
{
    [SerializeField] private int totalFireWood = 0;
    [SerializeField] private GameObject currentItem;

    public int TotalFireWood => totalFireWood;
    public GameObject CurrentItem => currentItem;


    public int ObtainFireWood()
    {
        return ++totalFireWood;

    }

    public GameObject ReplaceItem(GameObject newItem)
    {
        if (currentItem == newItem)
        {
            return currentItem;
        }

        GameObject oldItem = currentItem;
        currentItem = newItem.GetComponent<ItemComponent>().Store();
        if (oldItem != null)
        {
            oldItem.GetComponent<ItemComponent>().Drop();
        }
        return oldItem;
    }

    public int BurnInventory()
    {
        int totalValue = totalFireWood;
        if (currentItem != null)
        {
            totalValue++;
        }
        totalFireWood = 0;
        currentItem = null;
        return totalValue;
    }
}
