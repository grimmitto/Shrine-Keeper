using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : MonoBehaviour, IInteractable
{
    public List<InventorySlot> slots = new();
    public int chestSize = 12;


    public string InteractionText => "Open Chest";
    public bool StaysInteractable => true;

    

    public void Interact()
    {
        ChestUI.Instance.OpenChest(this);
    }

    public bool AddItem(IStorable newItem)
    {
        // Try stacking
        foreach (var slot in slots)
        {
            if (slot.item == newItem && slot.count < newItem.MaxStack)
            {
                slot.count++;
                ChestUI.Instance.RefreshUI();
                return true;
            }
        }

        // Add new slot
        if (slots.Count >= chestSize)
            return false;

        slots.Add(new InventorySlot { item = newItem, count = 1 });
        ChestUI.Instance.RefreshUI();
        return true;
    }

    public void RemoveItem(IStorable item)
    {
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

        ChestUI.Instance.RefreshUI();
    }

    public void RemoveItemCompletely(int index)
    {
        if (index >= 0 && index < slots.Count)
            slots.RemoveAt(index);

        ChestUI.Instance.RefreshUI();
    }

    public void PlaceItemAt(InventorySlot slot, int index)
    {
        if (index < slots.Count)
            slots[index] = slot;
        else
            slots.Add(slot);

        ChestUI.Instance.RefreshUI();
    }
}
