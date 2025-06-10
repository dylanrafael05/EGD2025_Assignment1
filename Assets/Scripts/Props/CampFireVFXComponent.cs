using UnityEngine;

public class CampFireVFXComponent : MonoBehaviour
{
    [SerializeField] private Light pointLight;



    void Awake()
    {
        pointLight = GetComponentInChildren<Light>();
    }



    public void SetLightIntensity(int lightValue)
    {
        pointLight.intensity = lightValue;
    }
}
