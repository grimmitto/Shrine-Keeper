using UnityEngine;
using System;
using System.Collections;
using Ink.Runtime;


public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance;

    [Header("Time Settings")]
    public float dayLengthInMinutes = 20f;
    public float currentTime = 7f / 24f; // Start at 7:00 AM
    public int dayNumber = 1;

    public bool isPaused;
    public float simulationTick = 1f;

    [Header("Managers")]
    public NeedsManager needsManager;

    public event Action OnNewDay;

    public bool isFastForwarding = false;

    public TextAsset progressionJSON;
    public Story progression;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (needsManager == null)
            needsManager = FindFirstObjectByType<NeedsManager>();

        if (GameManager.Instance != null && SaveUtility.SaveExists(GameManager.Instance.currentSlot) == false)
        {
            currentTime = 7f / 24f;
        }

        StartCoroutine(SimulationRoutine());

        // Also trigger weather for day 1
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.RollWeather();

        progression = new Story(progressionJSON.text);

        SimulationManager.Instance.OnNewDay += HandleNewDayInk;

    }

    private IEnumerator SimulationRoutine()
    {
        while (true)
        {
            if (!isPaused)
                UpdateTime();

            yield return new WaitForSeconds(simulationTick);
        }
    }

    private void UpdateTime()
    {
        float dayDurationSeconds = dayLengthInMinutes * 60f;
        float delta = simulationTick / dayDurationSeconds;
        currentTime += delta;

        if (currentTime >= 1f)
        {
            currentTime = 0f;
            dayNumber++;

            // Fire event
            OnNewDay?.Invoke();

            // Direct signal to WeatherManager
            if (WeatherManager.Instance != null)
                WeatherManager.Instance.RollWeather();
        }
    }

    // Helpers
    public float Get24HourTime() => currentTime * 24f;
    public int GetHour() => Mathf.FloorToInt(Get24HourTime());
    public int GetMinute() => Mathf.FloorToInt((Get24HourTime() % 1f) * 60f);

    public bool IsNight()
    {
        float hour = Get24HourTime();
        return hour >= 20f || hour < 5f;
    }

    public void SleepFastForward(float targetHour = 6f)
    {
        StartCoroutine(SleepFastForwardRoutine(targetHour));
    }

    private IEnumerator SleepFastForwardRoutine(float targetHour)
    {
        isFastForwarding = true;

        float originalTick = simulationTick;
        simulationTick = 0.01f;

        while (Get24HourTime() < targetHour)
            yield return null;

        currentTime = targetHour / 24f;

        simulationTick = originalTick;
        isFastForwarding = false;

        dayNumber++;
        OnNewDay?.Invoke();

        // Trigger weather after waking up
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.RollWeather();
    }

    void HandleNewDayInk()
    {
        progression.ChoosePathString("new_day");
        while (progression.canContinue)
            progression.Continue();

        ApplyInkValues();
    }

    private void ApplyInkValues()
    {
        if (progression == null) return;

        // --- Read variables from Ink ---
        int shrineLevel = (int)progression.variablesState["shrine_level"];
        int maintenance = (int)progression.variablesState["maintenance_state"];
        int reputation = (int)progression.variablesState["reputation"];
        int dailyVisitors = (int)progression.variablesState["daily_visitor_limit"];

        // --- Apply to your Unity systems ---
        // (These hooks won’t break anything if the managers don’t exist yet)

        // Example: shrine visuals
        if (ShrineManager.Instance != null)
            ShrineManager.Instance.SetShrineLevel(shrineLevel);

        ShrineManager.Instance.ApplyUnlocks(
        (bool)progression.variablesState["shop_unlocked"],
        (bool)progression.variablesState["crafting_unlocked"]
    );


        // Example: maintenance visuals
        if (MaintenanceManager.Instance != null)
            MaintenanceManager.Instance.SetMaintenance(maintenance);

        // Example: inform visitor manager
        if (VisitorManager.Instance != null)
            VisitorManager.Instance.SetDailyVisitorLimit(dailyVisitors);

        VisitorManager.Instance.SpawnVisitorsForDay();


        // Store for UI or debugging
        Debug.Log($"[INK] ShrineLevel={shrineLevel}, Maint={maintenance}, Rep={reputation}");
    }


}
