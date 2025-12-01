using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Slot References (same size for both lists)")]
    public List<Image> slotImages;
    public List<TMP_Text> slotCounts;

    [Header("Inventory Panel Root")]
    public GameObject inventoryPanel;

    public Sprite woodSprite;
    public Sprite stoneSprite;


    bool isOpen = false;

    void Start()
    {
        inventoryPanel.SetActive(false);
        RefreshUI();
    }

    // 🔥 Called by your Input System "OpenInventory" action
    public void OnInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)         // Button pressed/held
            OpenInventory();
        else if (ctx.canceled)     // Button released
            CloseInventory();
    }

    public void OpenInventory()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
        RefreshUI();
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }

    public void RefreshUI()
    {
        var slots = InventoryManager.Instance.slots;

        for (int i = 0; i < slotImages.Count; i++)
        {
            if (i < slots.Count && slots[i].item != null)
            {
                // Enable and assign sprite
                slotImages[i].enabled = true;
                slotImages[i].sprite = slots[i].item.InventorySprite;

                // Enable and assign count
                int count = slots[i].count;
                slotCounts[i].enabled = true;
                slotCounts[i].text = (count > 1) ? count.ToString() : "";
            }
            else
            {
                // Disable unused slots
                slotImages[i].enabled = false;
                slotCounts[i].enabled = false;
            }
        }
    }
}
