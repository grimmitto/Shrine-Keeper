using UnityEngine;
using System.Collections.Generic;

public enum PlayerGender { Male = 0, Female = 1 }

[System.Serializable]
public class SaveData
{
    public string playerName;
    public PlayerGender playerGender;
    // ---- SCENE + PLAYER POSITION ----
    public string lastScene;
    public float[] playerPosition = new float[3];
    public float[] playerRotation = new float[4];

    // ---- PLAYER NEEDS ----
    public float Hunger;
    public float Energy;
    public float Cleanliness;

    // ---- TIME + DAY ----
    public float currentTime;
    public int dayNumber;

    // ---- SHRINE VALUES ----
    public int reputation;
    public int renown;
    public int money;
    public int shrineLevel;

    // ---- MAINTENANCE ----
    public int maintenanceState;

    // ---- WEATHER ----
    public WeatherType weatherType;

    // ---- INVENTORY ----
    public List<string> inventoryItemIDs = new();
    public List<int> inventoryStackCounts = new();

    // ---- CHESTS ----
    public List<string> chestItemPrefabNames = new();
    public List<int> chestItemCounts = new();



    // ---- FUTURE SYSTEMS ----
    public List<string> activeVisitors = new();
    public Dictionary<string, bool> dialogueFlags = new();
}
