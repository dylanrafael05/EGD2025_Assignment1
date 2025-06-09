using System;
using UnityEditor.UIElements;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] private GameObject inspectElement;
    [SerializeField] private bool isStored = false;

    public GameObject InspectElement => inspectElement;


    void Awake()
    {

    }



    void Update()
    {
        if (isStored)
        {
            transform.position = PlayerManager.instance.transform.position + Vector3.down * 25;
        }
    }



    public GameObject Store()
    {
        isStored = true;
        return gameObject;
    }

    public GameObject Drop()
    {
        isStored = false;
        transform.position = PlayerManager.instance.transform.position;
        return gameObject;
    }
}
