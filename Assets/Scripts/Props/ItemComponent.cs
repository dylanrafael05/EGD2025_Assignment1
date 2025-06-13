using System;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public SpecialItem Specification { get; set; }

    public ItemComponent Store()
    {
        gameObject.SetActive(false);
        return this;
    }

    public ItemComponent Drop()
    {
        gameObject.SetActive(true);
        return this;
    }
}
