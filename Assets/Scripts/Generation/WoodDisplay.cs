using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WoodDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField] private PlayerInventoryComponent inventory;
    private int prevValue = -1;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (prevValue != inventory.TotalFireWood)
        {
            text.text = $"x{inventory.TotalFireWood}";
            prevValue = inventory.TotalFireWood;
        }
    }
}
