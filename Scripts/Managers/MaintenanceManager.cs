using UnityEngine;
using System.Collections;
using Ink.Runtime;

public class MaintenanceManager : MonoBehaviour
{
    public static MaintenanceManager Instance;

    [Header("Maintenance Debris States")]
    public GameObject cleanState;
    public GameObject slightlyDirtyState;
    public GameObject veryDirtyState;

    [Header("Debris Prefabs")]
    public GameObject[] fallenBranchPrefabs;
    public GameObject[] stonePrefabs;
    public GameObject[] dirtPilePrefabs;
    public GameObject[] driftwoodPrefabs;

    [Header("Spawn Zones")]
    public Transform[] shrineZoneSpawns;
    public Transform[] forestZoneSpawns;
    public Transform[] shoreZoneSpawns;

    [Header("Spawn Timing")]
    public float minSpawnDelay = 20f;
    public float maxSpawnDelay = 40f;

    [Header("Daily Maintenance Limits")]
    public int maxDailyMaintenance = 15;
    private int maintenanceGainedToday = 0;


    private Story progression;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        // 🔥 Wait for SimulationManager & Ink to initialize properly
        while (SimulationManager.Instance == null ||
               SimulationManager.Instance.progression == null)
        {
            yield return null;
        }

        progression = SimulationManager.Instance.progression;

        // Debug to confirm
        Debug.Log("<color=green>[Maintenance]</color> Ink progression ready.");

        // Begin spawning debris
        StartCoroutine(DebrisRoutine());
    }

    // --------------------------------------------------------------
    // VISUAL STATE TOGGLES (simple switch based on maintenance %)
    // --------------------------------------------------------------
    public void SetMaintenance(int maintenance)
    {
        cleanState?.SetActive(maintenance >= 70);
        slightlyDirtyState?.SetActive(maintenance < 70 && maintenance >= 35);
        veryDirtyState?.SetActive(maintenance < 35);
    }

    // --------------------------------------------------------------
    // RANDOM DEBRIS SPAWNER ROUTINE
    // --------------------------------------------------------------
    private IEnumerator DebrisRoutine()
    {
        // 🔥 Immediately spawn a piece of debris at game start
        TrySpawnDebris();

        // 🔁 Then begin the regular loop
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            TrySpawnDebris();
        }
    }

    // --------------------------------------------------------------
    // CORE SPAWN LOGIC
    // --------------------------------------------------------------
    private void TrySpawnDebris()
    {
        // Ensure progression still valid
        if (progression == null)
        {
            Debug.LogWarning("<color=yellow>[Maintenance]</color> Progression null, skipping debris spawn.");
            return;
        }

        // Choose zone
        int zoneRoll = Random.Range(0, 3);
        Transform[] zone = zoneRoll switch
        {
            0 => shrineZoneSpawns,
            1 => forestZoneSpawns,
            _ => shoreZoneSpawns
        };

        bool affectsShrine = (zoneRoll == 0);

        // Validate zone
        if (zone == null || zone.Length == 0)
        {
            Debug.LogWarning("<color=red>[Maintenance]</color> Spawn zone empty!");
            return;
        }

        // Pick random point
        Transform point = zone[Random.Range(0, zone.Length)];
        if (point == null)
        {
            Debug.LogWarning("<color=red>[Maintenance]</color> Spawn point missing!");
            return;
        }

        // Decide debris type
        GameObject[] chosenPool = zoneRoll == 0 ? SelectShrineDebrisType() : SelectRandomDebrisPool();

        if (chosenPool == null || chosenPool.Length == 0)
        {
            Debug.LogWarning("<color=red>[Maintenance]</color> No prefabs in selected debris pool!");
            return;
        }

        GameObject prefab = chosenPool[Random.Range(0, chosenPool.Length)];

        if (prefab == null)
        {
            Debug.LogWarning("<color=red>[Maintenance]</color> Debris prefab is NULL in the array!");
            return;
        }

        // Spawn it
        Instantiate(prefab, point.position, point.rotation);


        // Decrease shrine maintenance if appropriate
        if (affectsShrine)
        {
            int current = (int)progression.variablesState["maintenance_state"];
            current = Mathf.Max(current - Random.Range(1, 3), 0);

            progression.variablesState["maintenance_state"] = current;
            SetMaintenance(current);
        }
    }

    // --------------------------------------------------------------
    // DEBRIS POOL HELPERS
    // --------------------------------------------------------------
    private GameObject[] SelectShrineDebrisType()
    {
        // Weight debris around shrine differently if needed
        int roll = Random.Range(0, 4);
        return roll switch
        {
            0 => fallenBranchPrefabs,
            1 => dirtPilePrefabs,
            2 => driftwoodPrefabs,
            _ => stonePrefabs
        };
    }

    private GameObject[] SelectRandomDebrisPool()
    {
        int roll = Random.Range(0, 4);
        return roll switch
        {
            0 => fallenBranchPrefabs,
            1 => stonePrefabs,
            2 => dirtPilePrefabs,
            _ => driftwoodPrefabs
        };
    }

    public void AddMaintenance(int amount)
    {
        // If day cap reached, stop any increase
        if (maintenanceGainedToday >= maxDailyMaintenance)
            return;

        int allowed = Mathf.Min(amount, maxDailyMaintenance - maintenanceGainedToday);
        maintenanceGainedToday += allowed;

        var prog = SimulationManager.Instance.progression;

        int current = (int)prog.variablesState["maintenance_state"];
        current = Mathf.Clamp(current + allowed, 0, 100);

        // Apply values back to Ink
        prog.variablesState["maintenance_state"] = current;

        // Visual changes for dirtiness
        SetMaintenance(current);  // Updates debris state prefabs

        // Reputation directly tied to maintenance
        ShrineManager.Instance.reputation = current;
        ShrineManager.Instance.UpdateUI();
    }

    public void ResetDailyMaintenance()
    {
        maintenanceGainedToday = 0;
    }


}
