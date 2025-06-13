using UnityEngine;
using UnityEngine.UI;

public class InspectDisplay : MonoBehaviour
{
    public static InspectDisplay Instance { get; private set; }

    [SerializeField] GameObject inventoryContainer;
    [SerializeField] GameObject displayContainer;
    [SerializeField] Image display;
    [SerializeField] PlayerInventoryComponent inventory;

    void Awake()
    {
        Instance = this;
    }

    public bool IsInspecting
        => (InputManager.instance.InspectBool || ForceDisplay)
        && inventory.CurrentItem;
        
    public bool ForceDisplay { get; set; }

    void Update()
    {
        if (IsInspecting)
        {
            inventoryContainer.SetActive(false);
            displayContainer.SetActive(true);

            display.sprite = inventory.CurrentItem.Specification.inspectSprite;
        }
        else
        {
            inventoryContainer.SetActive(true);
            displayContainer.SetActive(false);
        }
    }
}