using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerInventoryComponent : MonoBehaviour
{
    [SerializeField] private int totalFireWood = 0;
    [SerializeField] private ItemComponent currentItem;

    public int TotalFireWood => totalFireWood;
    public ItemComponent CurrentItem => currentItem;


    public int ObtainFireWood()
    {
        return ++totalFireWood;

    }

    public ItemComponent ReplaceItem(ItemComponent newItem)
    {
        if (currentItem == newItem)
        {
            return currentItem;
        }

        var oldItem = currentItem;
        currentItem = newItem.Store();
        if (oldItem != null)
        {
            oldItem.Drop();
        }

        PostProcessEffects.Instance.CollectItem().Forget();

        return oldItem;
    }

    public int BurnInventory()
    {
        int totalValue = totalFireWood;
        if (currentItem != null)
        {
            totalValue++;
            SpecialItemPlacer.Instance.BurnSpecialItem();
        }
        totalFireWood = 0;
        currentItem = null;
        return totalValue;
    }
}
