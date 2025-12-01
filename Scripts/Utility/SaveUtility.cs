using UnityEngine;
using System.IO;

public static class SaveUtility
{
    public static void WriteSave(SaveData saveData, int saveSlot)
    {
        string saveDirectory = Path.Combine(Application.persistentDataPath, "SaveFiles");
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
        string slotPath = Path.Combine(saveDirectory, $"Save_{saveSlot}.json");
        string json = JsonUtility.ToJson(saveData, prettyPrint: true);

        File.WriteAllText(slotPath, json);
    }

    public static string[] GetAllSaves()
    {
        string saveDirectory = Path.Combine(Application.persistentDataPath, "SaveFiles");
        if (!Directory.Exists(saveDirectory)) return new string[0];
        return Directory.GetFiles(saveDirectory, "Save_*.json");
    }


    //Check if save exisits in slot
    public static bool SaveExists(int slot)
    {
        string slotPath = Path.Combine(Application.persistentDataPath, "SaveFiles", $"Save_{slot}.json");
        if (File.Exists(slotPath))
        {
            Debug.Log("Slot already used.");
        }
        else
        { Debug.Log("Empty slot."); }

        return File.Exists(slotPath);
    }

    public static SaveData ReadSave(int slot)
    {
        string slotPath = Path.Combine(Application.persistentDataPath, "SaveFiles", $"Save_{slot}.json");

        if (!File.Exists(slotPath))
        {
            Debug.LogWarning($"[SaveUtility] No save file found at slot {slot}");
            return null;
        }

        string json = File.ReadAllText(slotPath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"[SaveUtility] Loaded save slot {slot}");
        return data;
    }

    public static void DeleteSave(int slot)
    {
        string path = Path.Combine(Application.persistentDataPath, "SaveFiles", $"Save_{slot}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveUtility] Deleted save slot {slot}");
        }
        else
        {
            Debug.LogWarning($"[SaveUtility] Tried to delete slot {slot}, but no save file existed.");
        }
    }



}
