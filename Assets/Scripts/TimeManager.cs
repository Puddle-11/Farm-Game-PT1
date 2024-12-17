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
    public float filteredTimeOfDay;
    [SerializeField] private Vector2 minMaxSunHeight;
    [SerializeField] private Vector2 minMaxSunAngle;
    [SerializeField] private AnimationCurve sunAngleOverDay;
    [SerializeField] private AnimationCurve sunHeightOverDay;
    [SerializeField] private AnimationCurve sunIntensityOverDay;
    [SerializeField] private Gradient sunColorOverHeight;
    [SerializeField] private float timer;
    private void Update()
    {
        timer += Time.deltaTime % (dayLength * 60);
        currentTime = (timer / (dayLength * 60)) * 2 - 1;
        UpdateLights();
    }
    private void UpdateLights()
    {
        intensity = Mathf.Cos(((currentTime + 1) / 2) * Mathf.PI * 2);
        dayLight.intensity = sunIntensityOverDay.Evaluate(intensity) * DayIntensity;
        nightLight.intensity =NightIntensity;
        filteredTimeOfDay = (currentTime + 1) / 2;
        filteredTimeOfDay += 0.25f;
        filteredTimeOfDay %= 1;
        filteredTimeOfDay *= 2;
        filteredTimeOfDay = Mathf.Clamp(filteredTimeOfDay, 0, 1);



        //between 0 and 1 
        float temp = -Mathf.Cos(filteredTimeOfDay * Mathf.PI * 2);
        temp += 1;
        temp /= 2;



        float angle = Mathf.Lerp(minMaxSunHeight.x, minMaxSunHeight.y, temp);

        dayLight.color = sunColorOverHeight.Evaluate((angle - minMaxSunHeight.x) / (minMaxSunHeight.y - minMaxSunHeight.x));

        angle = sunHeightOverDay.Evaluate(filteredTimeOfDay) * (minMaxSunHeight.y - minMaxSunHeight.x) + minMaxSunHeight.x;
        float angle2 = sunAngleOverDay.Evaluate(filteredTimeOfDay) *(minMaxSunAngle.y - minMaxSunAngle.x) + minMaxSunAngle.x;
        dayLight.gameObject.transform.rotation = Quaternion.Euler(angle, angle2, 0);
    }



}
