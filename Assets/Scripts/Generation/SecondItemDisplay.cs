using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SecondItemDisplay : MonoBehaviour
{
    [SerializeField] private PlayerInventoryComponent inventory;
    Image uiImage;

    void Awake()
    {
        uiImage = GetComponent<Image>();
    }

    void Update()
    {
        var current = inventory.CurrentItem;
        if (current)
        {
            uiImage.sprite = current.Specification.smallSprite;
            uiImage.enabled = true;
        }
        else
        {
            uiImage.enabled = false;
        }
    }
}
