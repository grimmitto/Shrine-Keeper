using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    public TMP_Text clockText;

    void Update()
    {
        int h = SimulationManager.Instance.GetHour();
        int m = SimulationManager.Instance.GetMinute();

        clockText.text = $"{h:00}:{m:00}";
    }
}
