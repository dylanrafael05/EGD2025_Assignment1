using UnityEngine;

public abstract class PoolableBehaviour : MonoBehaviour
{
    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
