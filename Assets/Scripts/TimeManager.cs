using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
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
    public float filteredTimeOfNight;
    [SerializeField] private Vector2 minMaxSunHeight;
    [SerializeField] private Vector2 minMaxSunAngle;
    [SerializeField] private AnimationCurve sunAngleOverDay;
    [SerializeField] private AnimationCurve sunHeightOverDay;
    [SerializeField] private AnimationCurve sunIntensityOverDay;
    [SerializeField] private Gradient sunColorOverHeight;
    [SerializeField] private float timer;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.LogWarning("Two time managers found, Destroyed one at: " + gameObject.name);
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        timer %= (dayLength * 60);
        currentTime = (timer / (dayLength * 60)) * 2 - 1;
        UpdateLights();
    }
    private void UpdateLights()
    {


        intensity = Mathf.Cos(((currentTime + 1) / 2) * Mathf.PI * 2);

        dayLight.intensity = sunIntensityOverDay.Evaluate(intensity) * DayIntensity;

        filteredTimeOfDay = (currentTime + 1) / 2;
        filteredTimeOfDay += 0.25f;
        filteredTimeOfDay %= 1;
        filteredTimeOfDay *= 2;
        filteredTimeOfDay = Mathf.Clamp(filteredTimeOfDay, 0, 1);

        filteredTimeOfNight = (currentTime + 1) / 2;
        filteredTimeOfNight += 0.75f;
        filteredTimeOfNight %= 1;
        filteredTimeOfNight *= 2;
        filteredTimeOfNight = Mathf.Clamp(filteredTimeOfNight, 0, 1);

        float temp = -Mathf.Cos(filteredTimeOfDay * Mathf.PI * 2);
        temp += 1;
        temp /= 2;



        float sunHeight = sunHeightOverDay.Evaluate(filteredTimeOfDay) * (minMaxSunHeight.y - minMaxSunHeight.x) + minMaxSunHeight.x;
        dayLight.color = sunColorOverHeight.Evaluate((sunHeight - minMaxSunHeight.x) / (minMaxSunHeight.y - minMaxSunHeight.x));


        float sunAngle = sunAngleOverDay.Evaluate(filteredTimeOfDay) *(minMaxSunAngle.y - minMaxSunAngle.x) + minMaxSunAngle.x;

        float moonHeight = sunHeightOverDay.Evaluate(filteredTimeOfNight) * (minMaxSunHeight.y - minMaxSunHeight.x) + minMaxSunHeight.x;
        float moonAngle = sunAngleOverDay.Evaluate((filteredTimeOfNight - 1) * -1) * (minMaxSunAngle.y - minMaxSunAngle.x) + minMaxSunAngle.x;


        nightLight.intensity = sunIntensityOverDay.Evaluate((filteredTimeOfNight - 1) * -1) * NightIntensity;




        dayLight.gameObject.transform.rotation = Quaternion.Euler(sunHeight, sunAngle, 0);

        if (filteredTimeOfDay < 1 && filteredTimeOfDay > 0)
        {
            nightLight.gameObject.transform.rotation = Quaternion.Euler(sunHeight, sunAngle, 0);

        }
        else
        {

            nightLight.gameObject.transform.rotation = Quaternion.Euler(moonHeight, moonAngle, 0);
        }

    }



}
