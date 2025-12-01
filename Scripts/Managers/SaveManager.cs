using UnityEngine;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }


    [HideInInspector] public SaveData saveInstance; // Use SaveManager.Instance.saveInstance to get reference
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (saveInstance == null)
            saveInstance = new SaveData();
    }




    public void WriteDataFromSave(int slot)
    {
        SaveUtility.WriteSave(saveInstance, slot);
    }


    public void LoadDataFromSave(int slot)
    {
        SaveData loaded = SaveUtility.ReadSave(slot);

        if (loaded != null)
        {
            SaveManager.Instance.saveInstance = loaded;
        }

    }

}
