using UnityEngine;

public class Incinerate : MonoBehaviour
{
    [SerializeField] private float rCowerTime;
    [SerializeField] private float rStaggerTime;



    public Vector2 Final()
    {
        if (rCowerTime > 0)
        {
            return Cower();
        }
        else if (rStaggerTime > 0)
        {
            return Stagger();
        }

        CampFireManager.instance.fireStrength += Time.deltaTime;
        if ((transform.position - CampFireManager.instance.transform.position).magnitude < 2.5)
        {
            print("Explode!!!");
            Application.Quit(1);
        }

        return Allure();
    }

    public Vector2 Cower()
    {
        rCowerTime -= Time.deltaTime;
        return Vector2.down;
    }

    public Vector2 Stagger()
    {
        GetComponent<MovementComponent>().moveSpeed = 1.5f;
        rStaggerTime -= Time.deltaTime;
        return Vector2.zero;
    }

    public Vector2 Allure()
    {
        return Vector2.up;
    }
}
