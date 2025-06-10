using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public abstract class CompassBase : MonoBehaviour
{
    public abstract float2 GetTarget();

    void Update()
    {
        var loc = PlayerManager.instance.transform.position.tofloat3().xz;
        var tgt = GetTarget();

        var del = tgt - loc;
        del = del.Rotate2D(Mathf.Deg2Rad * Camera.main.transform.eulerAngles.y);

        transform.localEulerAngles = new(0, 0, Mathf.Rad2Deg * math.atan2(del.y, del.x));
    }
}
