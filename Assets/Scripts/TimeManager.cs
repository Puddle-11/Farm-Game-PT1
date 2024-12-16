using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TimeManager : MonoBehaviour
{
    public float currentTime; //Between -1 and 1, -1 and 1 being noon, 0 being midnight

    public float dayLength; //in minutes

    public Color DayColor;
    public Color NightColor;

    public float DayIntensity;
    public float NightIntensity;

    public Light nightLight;
    public Light dayLight;
    public float intensity;

    [SerializeField] private Vector2 minMaxSunHeight;

    private void Update()
    {
        currentTime = ((Time.time % (dayLength * 60)) / (dayLength * 60)) * 2 - 1;
        UpdateLights();
    }
    private void UpdateLights()
    {
        intensity = Mathf.Cos(((currentTime + 1) / 2) * Mathf.PI * 2);
        dayLight.intensity = intensity * DayIntensity;
        nightLight.intensity =NightIntensity;
        float filteredTime = (currentTime + 1) / 2; //time between 0 and 1,


        float step = Mathf.Clamp(currentTime, 0, 1);
        float angle = Mathf.Lerp(minMaxSunHeight.x, minMaxSunHeight.y, step);
        dayLight.gameObject.transform.rotation = Quaternion.Euler(angle, 0,0);
    }



}
