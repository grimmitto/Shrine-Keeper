using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;


public class InventorySlot
{
    public IStorable item;
    public int count;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private void Awake() => Instance = this;

    public Transform handMount;
    private GameObject currentEquipped;
    public InventoryUI inventoryUI;

    [SerializeField] public int slotCount = 6;
    public List<IStorable> items = new();       // used by other systems
    public List<InventorySlot> slots = new();   // used by UI / stacking


    public bool AddItem(IStorable newItem)
    {
        // Try stacking first
        foreach (var slot in slots)
        {
            if (slot.item == newItem && slot.count < newItem.MaxStack)
            {
                slot.count++;
                items.Add(newItem);  // keep legacy list in sync
                if (inventoryUI) inventoryUI.RefreshUI();
                return true;
            }
        }

        // Otherwise create a new slot
        if (slots.Count >= slotCount) return false;

        slots.Add(new InventorySlot { item = newItem, count = 1 });
        items.Add(newItem);  // sync legacy list
        if (inventoryUI) inventoryUI.RefreshUI();
        return true;
    }


    public void RemoveItem(IStorable item)
    {
        // Update stacked slots
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].item == item)
            {
                slots[i].count--;
                if (slots[i].count <= 0)
                    slots.RemoveAt(i);
                break;
            }
        }

        // Update legacy flat list
        items.Remove(item);

        if (inventoryUI) inventoryUI.RefreshUI();
    }


    public void EquipItem(IStorable item)
    {
        if (currentEquipped) Destroy(currentEquipped);

        if (item == null) return;

        currentEquipped = Instantiate(item.WorldPrefab, handMount);
        currentEquipped.transform.localPosition = Vector3.zero;
        currentEquipped.transform.localRotation = Quaternion.identity;
    }

    public void UnequipItem()
    {
        if (currentEquipped) Destroy(currentEquipped);
    }

    public void RemoveItemCompletely(int index)
    {
        if (index >= 0 && index < slots.Count)
            slots.RemoveAt(index);

        if (inventoryUI) inventoryUI.RefreshUI();
    }

    public void PlaceItemAt(InventorySlot slot, int index)
    {
        if (index < slots.Count)
            slots[index] = slot;
        else
            slots.Add(slot);

        if (inventoryUI) inventoryUI.RefreshUI();
    }


}

