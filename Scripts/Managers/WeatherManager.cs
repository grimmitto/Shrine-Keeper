using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance;

    [Header("Chances (0 to 1)")]
    public float sunnyChance = 0.5f;
    public float cloudyChance = 0.3f;
    public float fogChance = 0.1f;
    public float stormChance = 0.1f;

    [Header("References")]
    public Volume mainVolume;

    public VolumeProfile sunnyProfile;
    public VolumeProfile cloudyProfile;
    public VolumeProfile foggyProfile;
    public VolumeProfile stormProfile;

    public WeatherType currentWeather;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // Ensure the active profile is not null
        if (mainVolume == null || mainVolume.profile == null)
        {
            Debug.LogWarning("WeatherManager: No valid profile on scene load → rerolling weather.");
            RollWeather();
        }
    }




    // -------------------------
    //  WEATHER LOGIC
    // -------------------------
    public void RollWeather()
    {
        float roll = Random.value;

        if (roll < sunnyChance)
            SetWeather(WeatherType.Sunny);
        else if (roll < sunnyChance + cloudyChance)
            SetWeather(WeatherType.Cloudy);
        else if (roll < sunnyChance + cloudyChance + fogChance)
            SetWeather(WeatherType.Foggy);
        else
            SetWeather(WeatherType.Storm);
    }

    public void SetWeather(WeatherType newWeather)
    {
        currentWeather = newWeather;

        switch (newWeather)
        {
            case WeatherType.Sunny:
                mainVolume.profile = sunnyProfile;
                break;

            case WeatherType.Cloudy:
                mainVolume.profile = cloudyProfile;
                break;

            case WeatherType.Foggy:
                mainVolume.profile = foggyProfile;
                break;

            case WeatherType.Storm:
                mainVolume.profile = stormProfile;
                break;
        }

        Debug.Log("Today's weather: " + newWeather);
    }

    public bool HasProfileFor(WeatherType type)
    {
        switch (type)
        {
            case WeatherType.Sunny: return sunnyProfile != null;
            case WeatherType.Cloudy: return cloudyProfile != null;
            case WeatherType.Foggy: return foggyProfile != null;
            case WeatherType.Storm: return stormProfile != null;
        }
        return false;
    }


}

public enum WeatherType
{
    Sunny,
    Cloudy,
    Foggy,
    Storm
}
