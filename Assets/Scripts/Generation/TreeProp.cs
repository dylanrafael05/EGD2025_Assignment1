using UnityEngine;

public class TreeProp : MonoBehaviour
{
    public void ChopDown()
    {
        gameObject.SetActive(false);
        GeneratorManager.Instance.KillForestAt(transform.position.tofloat3().xz);
    }
}
