using UnityEngine;
using System.Collections;

public class NeedsManager : MonoBehaviour
{
    [Header("References")]
    public SimulationManager simulationManager;
    public NeedsUI needsUI;

    public static NeedsManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [HideInInspector] public bool isPaused;

    [Header("Needs (0–100)")]
    [Range(0, 100)] public float Hunger = 100f;
    [Range(0, 100)] public float Energy = 100f;
    [Range(0, 100)] public float Cleanliness = 100f;

    void Start()
    {
        if (simulationManager == null)
            simulationManager = FindFirstObjectByType<SimulationManager>();

        if (needsUI == null)
            needsUI = FindFirstObjectByType<NeedsUI>();

        Hunger = Energy = Cleanliness = 100f;

        needsUI.SetTargets(Hunger, Energy, Cleanliness);

        StartCoroutine(NeedsRoutine());
    }


    private IEnumerator NeedsRoutine()
    {
        while (true)
        {
            if (!isPaused)
            {
                if (Hunger > 0) Hunger -= 4f;
                if (Energy > 0) Energy -= 2f;
                if (Cleanliness > 0) Cleanliness -= 1f;

                // Update UI targets once per tick
                needsUI.SetTargets(Hunger, Energy, Cleanliness);
            }

            yield return new WaitForSeconds(60f * simulationManager.simulationTick);
        }
    }

    // --- Interactions ---
    public void Eat(float nutritionalValue)
    {
        Hunger = Mathf.Clamp(Hunger + nutritionalValue, 0, 100f);
        needsUI.SetTargets(Hunger, Energy, Cleanliness);
    }

    public void CleanSelf(float amount)
    {
        Cleanliness = Mathf.Clamp(Cleanliness + amount, 0, 100f);
        needsUI.SetTargets(Hunger, Energy, Cleanliness);
    }

    public void Sleep()
    {
        Energy = 100f;
        needsUI.SetTargets(Hunger, Energy, Cleanliness);
    }
}
