using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CampFireManager : MonoBehaviour
{
    public static CampFireManager instance;

    [NonSerialized] private CampFireVFXComponent campFireVFXComponent;
    [SerializeField] private Animator campFireAnimatorComponent;
    [NonSerialized] private int fireStrength;

    public int TotalUsedLogs => fireStrength;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        campFireVFXComponent = GetComponent<CampFireVFXComponent>();
    }



    void Update()
    {
        campFireVFXComponent.SetLightIntensity(fireStrength + Random.Range(-1f,1f));
    }



    public void IncreaseBurn(int incrementValue)
    {
        fireStrength += incrementValue;
        campFireAnimatorComponent.SetTrigger("fanned");
    }
}
