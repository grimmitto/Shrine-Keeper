using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string newGameSceneName = "NewGame";
    public string persistentSceneName = "RootScene";
    public string mainMenuScene = "MainMenu";
    [HideInInspector] public int currentSlot;

    private bool isLoading = false;

    public PlayerGender playerGender;


    // -----------------------------------------------------------
    //  AWAKE / START
    // -----------------------------------------------------------
    void Awake()
    {
        Debug.Log("<color=orange>GM Awake on object: " + gameObject.name + "</color>");
        Debug.Log("newGameSceneName at Awake = " + newGameSceneName);
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(EnsureMainMenuLoaded());
    }

    IEnumerator EnsureMainMenuLoaded()
    {
        yield return null;

        // Only act if RootScene is the ONLY loaded scene
        if (SceneManager.sceneCount != 1)
        {
            // More than one scene already loaded → do nothing
            yield break;
        }

        Scene onlyScene = SceneManager.GetSceneAt(0);

        if (onlyScene.name != persistentSceneName)
        {
            // The only scene is NOT RootScene → do nothing
            yield break;
        }

        // Now we know:
        // ✔ Only one scene is loaded
        // ✔ That scene is RootScene
        // → We must load MainMenu

        Debug.Log("<color=cyan>RootScene is alone — loading MainMenu…</color>");

        yield return SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);

        Scene menu = SceneManager.GetSceneByName(mainMenuScene);

        if (menu.IsValid())
        {
            SceneManager.SetActiveScene(menu);
            Debug.Log("<color=green>MainMenu is now active.</color>");
        }
        else
        {
            Debug.LogError("MainMenu STILL not loaded. Something else is interfering.");
        }

        StartCoroutine(DelayActionMapSwitch("UI"));
    }



    IEnumerator DelayActionMapSwitch(string map)
    {
        yield return null;

        var input = FindFirstObjectByType<PlayerInput>();
        if (input != null)
            input.SwitchCurrentActionMap(map);
    }


    // -----------------------------------------------------------
    //  PUBLIC START/LOAD FUNCTIONS
    // -----------------------------------------------------------
    public void SetSlot(int newSlot) => currentSlot = newSlot;

    public void NewGame()
    {

        SaveManager.Instance.saveInstance = new SaveData();

        var save = SaveManager.Instance.saveInstance;

        save.lastScene = newGameSceneName;

        Debug.Log("<color=lime>--- ENTERED NewGame() ---</color>");
        Debug.Log("Inspector value of newGameSceneName = " + newGameSceneName);
        Debug.Log("Slot says SaveExists = " + SaveUtility.SaveExists(currentSlot));


        save.Hunger = 100f;
        save.Energy = 100f;
        save.Cleanliness = 100f;

        save.currentTime = 7f / 24f;
        save.dayNumber = 1;

        save.reputation = 0;
        save.renown = 0;
        save.money = 0;

        save.shrineLevel = 0;
        save.maintenanceState = 0;

        save.weatherType = WeatherType.Sunny;

        Debug.Log("<color=lime>NEW GAME STARTED → Forcing load of: " + newGameSceneName + "</color>");


        StartCoroutine(SwapScene(newGameSceneName));
    }


    public void LoadGame()
    {
        SaveManager.Instance.LoadDataFromSave(currentSlot);

        string lastScene = SaveManager.Instance.saveInstance.lastScene;
        StartCoroutine(SwapScene(lastScene));
    }

    IEnumerator WaitAndSwap(string sceneName)
    {
        // wait until current load is done
        while (isLoading)
            yield return null;

        StartCoroutine(SwapScene(sceneName));
    }


    // -----------------------------------------------------------
    //  SCENE LOADING (SAFE ORDER)
    // -----------------------------------------------------------
    public IEnumerator SwapScene(string sceneName)
    {
        // NEW VERSION: Queue scene loads safely
        if (isLoading)
        {
            Debug.Log("<color=yellow>Scene load requested while already loading. Queueing...</color>");
            StartCoroutine(WaitAndSwap(sceneName));
            yield break;
        }

        Debug.Log("<color=yellow>SwapScene CALLED with: " + sceneName + "</color>");


        isLoading = true;

        Scene previousScene = SceneManager.GetActiveScene();

        Debug.Log($"<color=cyan>=== SWAP SCENE START ===</color>");
        Debug.Log($"Active BEFORE load: {previousScene.name}");
        Debug.Log($"Target new scene: {sceneName}");

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return load;

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        // ⭐ NEW — spawn the player for this scene
        var spawner = FindObjectOfType<PlayerSpawner>();
        if (spawner != null)
        {
            spawner.SpawnPlayer(SaveManager.Instance.saveInstance);
        }
        else
        {
            Debug.LogWarning("No PlayerSpawner found in scene: " + newScene.name);
        }

        StartCoroutine(DelayActionMapSwitch("Player"));


        if (previousScene.name != persistentSceneName &&
            previousScene.name != newScene.name)
        {
            Debug.Log($"<color=red>Attempting to UNLOAD: {previousScene.name}</color>");

            AsyncOperation unload = SceneManager.UnloadSceneAsync(previousScene);

            if (unload == null)
            {
                Debug.LogError($"<color=red>Unload FAILED: unload operation returned NULL for {previousScene.name} </color>");
            }
            else
            {
                yield return unload;
                Debug.Log($"<color=magenta>UNLOADED SCENE: {previousScene.name}</color>");
            }
        }
        else
        {
            Debug.Log($"<color=yellow>Not unloading scene because it's either root or same scene.</color>");
        }

        yield return null;
        yield return null;

        Debug.Log("<color=cyan>Applying Loaded Data…</color>");
        ApplyLoadedData();

        Debug.Log("<color=cyan>=== SWAP SCENE COMPLETE ===</color>");

        isLoading = false;
    }



    // -----------------------------------------------------------
    //  APPLY SAVE DATA (FULL VERSION)
    // -----------------------------------------------------------
    public void ApplyLoadedData()
    {
        var save = SaveManager.Instance.saveInstance;
        if (save == null)
        {
            Debug.LogWarning("ApplyLoadedData() called but no save exists.");
            return;
        }

        string activeScene = SceneManager.GetActiveScene().name;

        // ---------------------------------------------------------
        // 1. ***NEVER APPLY SAVE DATA IN NEW GAME SCENE***
        // ---------------------------------------------------------
        if (activeScene == newGameSceneName)
        {
            Debug.Log("ApplyLoadedData skipped — NewGame scene detected.");
            return;
        }

        // ---------------------------------------------------------
        // 2. ***ONLY APPLY POSITION/ROTATION IF A SAVE FILE ACTUALLY EXISTS***
        // ---------------------------------------------------------
        bool hasValidSave = SaveUtility.SaveExists(currentSlot);

        if (!hasValidSave)
        {
            Debug.Log("ApplyLoadedData skipped — no valid save exists yet.");
            return;
        }

        // ---------------------------------------------------------
        // 3. FIND PLAYER — if missing, retry
        // ---------------------------------------------------------
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("ApplyLoadedData() → Player not found. Retrying...");
            StartCoroutine(DelayedApply());
            return;
        }

        // ---------------------------------------------------------
        // 4. Restore saved position
        // ---------------------------------------------------------
        Vector3 pos = new Vector3(
            save.playerPosition[0],
            save.playerPosition[1],
            save.playerPosition[2]
        );

        Quaternion rot = new Quaternion(
            save.playerRotation[0],
            save.playerRotation[1],
            save.playerRotation[2],
            save.playerRotation[3]
        );

        // Hard guard against corrupt save or uninitialized value
        if (pos == Vector3.zero)
        {
            Debug.LogWarning("Saved position is ZERO — skipping teleport. Using scene spawn instead.");
        }
        else
        {
            Debug.Log("[ApplyLoadedData] Restoring player pos: " + pos);

            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            player.transform.SetPositionAndRotation(pos, rot);

            if (cc) cc.enabled = true;
        }

        // ---------------------------------------------------------
        // 5. Apply NEEDS
        // ---------------------------------------------------------
        if (NeedsManager.Instance != null)
        {
            NeedsManager.Instance.Hunger = save.Hunger;
            NeedsManager.Instance.Energy = save.Energy;
            NeedsManager.Instance.Cleanliness = save.Cleanliness;

            if (NeedsManager.Instance.needsUI != null)
                NeedsManager.Instance.needsUI.SetTargets(
                    save.Hunger, save.Energy, save.Cleanliness
                );
        }

        // ---------------------------------------------------------
        // 6. Apply TIME + DAY
        // ---------------------------------------------------------
        if (SimulationManager.Instance != null)
        {
            SimulationManager.Instance.currentTime = save.currentTime;
            SimulationManager.Instance.dayNumber = save.dayNumber;

            if (SimulationManager.Instance.progression != null)
            {
                SimulationManager.Instance.progression.variablesState["maintenance_state"] = save.maintenanceState;
                SimulationManager.Instance.progression.variablesState["shrine_level"] = save.shrineLevel;
            }
        }

        // ---------------------------------------------------------
        // 7. Apply SHRINE progress
        // ---------------------------------------------------------
        if (ShrineManager.Instance != null)
        {
            ShrineManager.Instance.reputation = save.reputation;
            ShrineManager.Instance.renown = save.renown;
            ShrineManager.Instance.money = save.money;

            ShrineManager.Instance.SetShrineLevel(save.shrineLevel);
            ShrineManager.Instance.UpdateUI();
        }

        // ---------------------------------------------------------
        // 8. Apply WEATHER
        // ---------------------------------------------------------
        if (WeatherManager.Instance != null)
        {
            bool validEnum = System.Enum.IsDefined(typeof(WeatherType), save.weatherType);

            if (!validEnum)
            {
                Debug.LogWarning("Saved weather invalid — rerolling.");
                WeatherManager.Instance.RollWeather();
            }
            else if (WeatherManager.Instance.HasProfileFor(save.weatherType))
            {
                WeatherManager.Instance.SetWeather(save.weatherType);
            }
            else
            {
                Debug.LogWarning("Saved weather profile missing — rerolling.");
                WeatherManager.Instance.RollWeather();
            }
        }

        // ---------------------------------------------------------
        // 9. Apply INVENTORY
        // ---------------------------------------------------------
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.slots.Clear();
            InventoryManager.Instance.items.Clear();

            for (int i = 0; i < save.inventoryItemIDs.Count; i++)
            {
                string itemName = save.inventoryItemIDs[i];
                int count = save.inventoryStackCounts[i];

                IStorable item = ItemRegistry.Get(itemName);
                if (item == null) continue;

                InventorySlot slot = new InventorySlot
                {
                    item = item,
                    count = count
                };

                InventoryManager.Instance.slots.Add(slot);
                InventoryManager.Instance.items.Add(item);
            }

            if (InventoryManager.Instance.inventoryUI != null)
                InventoryManager.Instance.inventoryUI.RefreshUI();
        }

        // ---------------------------------------------------------
        // 10. Chest Inventory
        // ---------------------------------------------------------
        var chest = GameObject.FindAnyObjectByType<ChestInventory>();
        if (chest != null)
        {
            chest.slots.Clear();

            for (int i = 0; i < save.chestItemPrefabNames.Count; i++)
            {
                string name = save.chestItemPrefabNames[i];
                int count = save.chestItemCounts[i];

                IStorable item = ItemRegistry.Get(name);
                if (item == null) continue;

                chest.slots.Add(new InventorySlot { item = item, count = count });
            }

            if (ChestUI.Instance != null)
                ChestUI.Instance.RefreshUI();
        }
    }



    IEnumerator DelayedApply()
    {
        yield return new WaitForSeconds(0.1f);
        ApplyLoadedData();
    }


    // -----------------------------------------------------------
    //  SAVE GAME
    // -----------------------------------------------------------
    public void SaveGame()
    {
        var save = SaveManager.Instance.saveInstance;

        if (save == null)
        {
            save = new SaveData();
            SaveManager.Instance.saveInstance = save;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var pos = player.transform.position;
            Debug.Log("[SaveGame] Saving pos: " + pos);
            Debug.Log("[SaveGame] Found player object: " + player.name);

            Quaternion rot = player.transform.rotation;

            save.playerPosition[0] = pos.x;
            save.playerPosition[1] = pos.y;
            save.playerPosition[2] = pos.z;

            save.playerRotation[0] = rot.x;
            save.playerRotation[1] = rot.y;
            save.playerRotation[2] = rot.z;
            save.playerRotation[3] = rot.w;
        }

        save.lastScene = SceneManager.GetActiveScene().name;

        if (NeedsManager.Instance != null)
        {
            save.Hunger = NeedsManager.Instance.Hunger;
            save.Energy = NeedsManager.Instance.Energy;
            save.Cleanliness = NeedsManager.Instance.Cleanliness;
        }

        if (SimulationManager.Instance != null)
        {
            save.currentTime = SimulationManager.Instance.currentTime;
            save.dayNumber = SimulationManager.Instance.dayNumber;

            if (SimulationManager.Instance.progression != null)
            {
                save.shrineLevel = (int)SimulationManager.Instance.progression.variablesState["shrine_level"];
                save.maintenanceState = (int)SimulationManager.Instance.progression.variablesState["maintenance_state"];
            }
        }

        if (ShrineManager.Instance != null)
        {
            save.reputation = ShrineManager.Instance.reputation;
            save.renown = ShrineManager.Instance.renown;
            save.money = ShrineManager.Instance.money;
        }

        if (WeatherManager.Instance != null)
            save.weatherType = WeatherManager.Instance.currentWeather;

        save.inventoryItemIDs.Clear();
        save.inventoryStackCounts.Clear();

        if (InventoryManager.Instance != null)
        {
            foreach (var slot in InventoryManager.Instance.slots)
            {
                if (slot.item == null) continue;

                save.inventoryItemIDs.Add(slot.item.ItemName);
                save.inventoryStackCounts.Add(slot.count);
            }
        }

        save.chestItemPrefabNames.Clear();
        save.chestItemCounts.Clear();

        var chest = GameObject.FindAnyObjectByType<ChestInventory>();
        if (chest != null)
        {
            foreach (var slot in chest.slots)
            {
                if (slot.item == null) continue;

                save.chestItemPrefabNames.Add(slot.item.ItemName);
                save.chestItemCounts.Add(slot.count);
            }
        }

        SaveManager.Instance.WriteDataFromSave(currentSlot);
    }


    // -----------------------------------------------------------
    //  EXIT GAME
    // -----------------------------------------------------------
    public void ExitGame()
    {
        SaveGame();
        Application.Quit();
    }
}
