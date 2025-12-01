using UnityEngine;

public class ShrineManager : MonoBehaviour
{
    public static ShrineManager Instance;

    [Header("Shrine Level Prefabs")]
    public GameObject level0Shrine;   // abandoned
    public GameObject level1Shrine;   // restored
    public GameObject level2Shrine;   // expanded

    [Header("Unlockable Objects")]
    public GameObject shopObject;
    public GameObject craftingStation;

    public int reputation = 0;  // 0–100
    public int renown = 0;      // 0–100
    public int money = 0;       // player yen

    


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        reputation = 0;
        renown = 0;
        money = 0;
        UpdateUI();
    }


    public void UpdateUI()
    {
        ShrineUI.Instance.UpdateUI(reputation, renown, money);
    }


    public void SetShrineLevel(int level)
    {
        // toggle different visual stages
        if (level0Shrine) level0Shrine.SetActive(level == 0);
        if (level1Shrine) level1Shrine.SetActive(level == 1);
        if (level2Shrine) level2Shrine.SetActive(level >= 2);
    }

    public void ApplyUnlocks(bool shopUnlocked, bool craftingUnlocked)
    {
        if (shopObject) shopObject.SetActive(shopUnlocked);
        if (craftingStation) craftingStation.SetActive(craftingUnlocked);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount) return false;
        money -= amount;
        UpdateUI();
        return true;
    }

}
