using System;
using TMPro;
using UnityEngine;

public class DebugLabel : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    public void Setup(GameObject follow, Vector3 offset, Func<string> textUpdator, Func<bool> shouldShow)
    {
        this.followObject = follow;
        this.offset = offset;
        this.textUpdator = textUpdator;
        this.shouldShow = shouldShow;
    }

    private Func<string> textUpdator;
    private Func<bool> shouldShow;
    private GameObject followObject;
    private Vector3 offset;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && shouldShow())
        {
            text.enabled = true;
            text.color = Color.white;
            text.text = textUpdator();

            transform.position = followObject.transform.position + offset;
            transform.forward = Camera.main.transform.forward;
        }
        else
        {
            text.enabled = false;
            text.color = Color.clear;
        }
    }
}
