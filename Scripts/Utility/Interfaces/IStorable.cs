using System.Collections.Generic;
using UnityEngine;

public interface IStorable
{

    Sprite InventorySprite { get; }
    int MaxStack { get; }
    GameObject WorldPrefab { get; }   // model to spawn when equipped
    string ItemName { get; }          // for debugging / display
}

public static class ItemRegistry
{
    private static Dictionary<string, IStorable> items = new();

    public static void Register(IStorable item)
    {
        if (item == null) return;
        string key = item.ItemName;


        if (!items.ContainsKey(key))
            items[key] = item;
    }

    public static IStorable Get(string name)
    {
        if (items.TryGetValue(name, out var item))
            return item;

        Debug.LogWarning($"[ItemRegistry] No item registered with name '{name}'");
        return null;
    }
}

