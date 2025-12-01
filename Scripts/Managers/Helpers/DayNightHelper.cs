using UnityEngine;
using System;

public class DayNightHelper : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;
    public Light sun;
    public Light moon;

    [Header("Rotation Settings")]
    public float sunriseHour = 7f;
    public float sunsetHour = 20f;
    public float rotationSmooth = 2f;

    [Header("Sun Lighting Points")]
    public TimeLightingPoint[] sunPoints;

    [Header("Moon Lighting Points")]
    public TimeLightingPoint[] moonPoints;

    private float smoothAngle;

    void Update()
    {
        float hour = SimulationManager.Instance.Get24HourTime();

        // ---- ROTATION ----
        float targetAngle = CalculateAngleForHour(hour);
        smoothAngle = Mathf.LerpAngle(smoothAngle, targetAngle, Time.deltaTime * rotationSmooth);
        pivot.rotation = Quaternion.Euler(smoothAngle, 0f, 0f);

        // ---- SUN ----
        ApplyLightFromPoints(hour, sun, sunPoints);

        // ---- MOON ----
        ApplyLightFromPoints(hour, moon, moonPoints);
    }

    // -----------------------------
    //        CORE HELPERS
    // -----------------------------

    void ApplyLightFromPoints(float hour, Light light, TimeLightingPoint[] points)
    {
        if (points == null || points.Length == 0)
            return;

        // Make sure list is sorted by time
        Array.Sort(points, (a, b) => a.hour.CompareTo(b.hour));

        // Wrap-around search (23 → 0)
        TimeLightingPoint a = points[0];
        TimeLightingPoint b = points[points.Length - 1];

        // Default: find two points A and B surrounding current time
        for (int i = 0; i < points.Length; i++)
        {
            TimeLightingPoint current = points[i];
            TimeLightingPoint next = points[(i + 1) % points.Length];

            bool isLastPair = (i == points.Length - 1);

            if (isLastPair)
            {
                // Pair is last → first (wrap)
                if (hour >= current.hour || hour < next.hour)
                {
                    a = current;
                    b = next;
                    break;
                }
            }
            else
            {
                if (hour >= current.hour && hour < next.hour)
                {
                    a = current;
                    b = next;
                    break;
                }
            }
        }

        // ---- INTERPOLATION ----
        float t = InverseLerpWrapped(a.hour, b.hour, hour);
        light.intensity = Mathf.Lerp(a.intensity, b.intensity, t);
        light.colorTemperature = Mathf.Lerp(a.temperature, b.temperature, t);
    }

    float InverseLerpWrapped(float a, float b, float v)
    {
        // Handles wrap-around between 23 → 0
        if (b < a) b += 24f;
        if (v < a) v += 24f;
        return Mathf.InverseLerp(a, b, v);
    }

    float CalculateAngleForHour(float hour)
    {
        // Day: sunrise → sunset = 0° → 180°
        if (hour >= sunriseHour && hour <= sunsetHour)
        {
            float t = Mathf.InverseLerp(sunriseHour, sunsetHour, hour);
            return Mathf.Lerp(0f, 180f, t);
        }

        // Night: sunset → next sunrise = 180° → 360°
        float adjusted = hour < sunriseHour ? hour + 24f : hour;
        float nt = Mathf.InverseLerp(sunsetHour, sunriseHour + 24f, adjusted);
        float ang = Mathf.Lerp(180f, 360f, nt);
        return (ang >= 360f) ? ang - 360f : ang;
    }
}

[Serializable]
public struct TimeLightingPoint
{
    [Range(0f, 24f)]
    public float hour;         // Exact clock time
    public float intensity;    // Light intensity
    public float temperature;  // Color temperature (Kelvin!)
}
