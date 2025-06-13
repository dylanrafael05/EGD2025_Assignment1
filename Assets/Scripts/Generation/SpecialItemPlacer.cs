using Unity.Mathematics;
using UnityEngine;

public class SpecialItemPlacer : ScenePropPlacer
{
    public static SpecialItemPlacer Instance { get; private set; }

    protected new void Awake()
    {
        base.Awake();
        Instance = this;
    }

    [SerializeField] private SpecialItemEntry[] entries;
    [SerializeField] private float chanceToPlace = 0.2f;
    [SerializeField] private PlayerInventoryComponent inventory;

    public void BurnSpecialItem()
    {
        index++;
        isPlaced = false;
        location = new();
    }

    private bool isPlaced = false;
    private float2 location;
    private int index = 0;

    public float2? ItemPlace
        => isPlaced ? location : null;

    public override void PlaceInChunk(ChunkInstance chunk)
    {
        if (!CampFireManager.instance)
            return;

        if (index >= entries.Length)
            return;

        var logs = CampFireManager.instance.TotalUsedLogs;
        if (logs < entries[index].minFirewood)
            return;

        if (isPlaced)
        {
            if (chunk.Bounds.Contains(location) && !inventory.CurrentItem)
            {
                var prop = AttemptCreate(chunk, location, false);
                prop.GetComponent<ItemComponent>().Specification = entries[index].specification;
            }
        }
        else if (UnityEngine.Random.value < chanceToPlace
             && chunk.Bounds.center.magnitude < GeneratorManager.Instance.VoidRadius)
        {
            isPlaced = true;
            location = chunk.Bounds.center;

            var prop = AttemptCreate(chunk, location, false);
            prop.GetComponent<ItemComponent>().Specification = entries[index].specification;
        }
    }
}
