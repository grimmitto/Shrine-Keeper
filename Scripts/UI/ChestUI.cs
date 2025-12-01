using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChestUI : MonoBehaviour
{
    public static ChestUI Instance;

    [Header("Held Icon Offset")]
    public Vector2 heldIconOffset = new Vector2(20f, -20f);


    void Awake() => Instance = this;

    public GameObject panel;


    private ChestInventory activeChest;

    [Header("Player Inventory UI")]
    public List<Image> playerSlotImages;
    public List<TMP_Text> playerSlotCounts;

    [Header("Chest UI")]
    public List<Image> chestSlotImages;
    public List<TMP_Text> chestSlotCounts;

    [Header("Held Item")]
    public Image heldItemImage;
    public TMP_Text heldItemCount;
    private InventorySlot heldSlot = null;

    private PlayerInput GetPlayerInput()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return null;

        return player.GetComponent<PlayerInput>();
    }


    void Start()
    {
        panel.SetActive(false);
        ClearHeldItem();
    }

    public void OpenChest(ChestInventory chest)
    {
        activeChest = chest;
        panel.SetActive(true);
        RefreshUI();

        var input = GetPlayerInput();
        if (input != null)
            input.SwitchCurrentActionMap("UI");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseChest()
    {
        panel.SetActive(false);
        activeChest = null;

        ClearHeldItem();

        var input = GetPlayerInput();
        if (input != null)
            input.SwitchCurrentActionMap("Player");

        InteractionManager.Instance.ReactivateUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefreshUI()
    {
        if (activeChest == null) return;

        UpdateSlotUI(playerSlotImages, playerSlotCounts, InventoryManager.Instance.slots);
        UpdateSlotUI(chestSlotImages, chestSlotCounts, activeChest.slots);
    }

    private void UpdateSlotUI(List<Image> imgs, List<TMP_Text> counts, List<InventorySlot> slots)
    {
        for (int i = 0; i < imgs.Count; i++)
        {
            if (slots != null && i < slots.Count && slots[i].item != null)
            {
                imgs[i].enabled = true;
                imgs[i].sprite = slots[i].item.InventorySprite;

                counts[i].enabled = true;
                counts[i].text = (slots[i].count > 1) ? slots[i].count.ToString() : "";
            }
            else
            {
                imgs[i].enabled = false;
                counts[i].enabled = false;
            }
        }
    }

    void Update()
    {
        if (heldSlot != null)
            UpdateHeldItemUI();
    }

    private void UpdateHeldItemUI()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            heldItemImage.canvas.transform as RectTransform,
            mousePos,
            heldItemImage.canvas.worldCamera,
            out Vector2 localPos
        );

        localPos += heldIconOffset;

        heldItemImage.rectTransform.anchoredPosition = localPos;
        heldItemCount.rectTransform.anchoredPosition = localPos;
    }


    private void ClearHeldItem()
    {
        heldSlot = null;
        heldItemImage.enabled = false;
        heldItemCount.enabled = false;
    }
    private void SetHeldItem(InventorySlot s)
    {
        heldSlot = new InventorySlot { item = s.item, count = s.count };

        heldItemImage.gameObject.SetActive(true);
        heldItemCount.gameObject.SetActive(true);

        heldItemImage.enabled = true;   // ← REQUIRED
        heldItemCount.enabled = true;   // ← REQUIRED

        heldItemImage.sprite = heldSlot.item.InventorySprite;
        heldItemCount.text = (heldSlot.count > 1) ? heldSlot.count.ToString() : "";
    }



    // LEFT CLICK = pick up/put entire stack
    public void OnLeftClickSlot(SlotButton.SlotType type, int index)
    {
        var source = (type == SlotButton.SlotType.Player)
            ? InventoryManager.Instance.slots
            : activeChest.slots;

        InventorySlot clicked = (index < source.Count) ? source[index] : null;

        // Case 1: picking up stack
        if (heldSlot == null)
        {
            if (clicked == null) return;

            SetHeldItem(clicked);

            // Remove from source
            if (type == SlotButton.SlotType.Player)
                InventoryManager.Instance.RemoveItemCompletely(index);
            else
                activeChest.RemoveItemCompletely(index);

            RefreshUI();
            return;
        }

        // Case 2: placing into empty
        if (clicked == null)
        {
            if (type == SlotButton.SlotType.Player)
                InventoryManager.Instance.PlaceItemAt(heldSlot, index);
            else
                activeChest.PlaceItemAt(heldSlot, index);

            ClearHeldItem();
            RefreshUI();
            return;
        }

        // Case 3: stacking
        if (clicked.item == heldSlot.item && clicked.count < clicked.item.MaxStack)
        {
            int add = Mathf.Min(heldSlot.count, clicked.item.MaxStack - clicked.count);
            clicked.count += add;
            heldSlot.count -= add;

            if (heldSlot.count <= 0)
                ClearHeldItem();

            RefreshUI();
            return;
        }

        // Case 4: swap
        InventorySlot temp = new InventorySlot { item = clicked.item, count = clicked.count };
        clicked.item = heldSlot.item;
        clicked.count = heldSlot.count;

        heldSlot = temp;

        RefreshUI();
    }

    // RIGHT CLICK = move one item
    public void OnRightClickSlot(SlotButton.SlotType type, int index)
    {
        var source = (type == SlotButton.SlotType.Player)
            ? InventoryManager.Instance.slots
            : activeChest.slots;

        InventorySlot clicked = (index < source.Count) ? source[index] : null;

        // Pick up one
        if (heldSlot == null)
        {
            if (clicked == null) return;

            SetHeldItem(new InventorySlot { item = clicked.item, count = 1 });

            clicked.count--;
            if (clicked.count <= 0)
            {
                if (type == SlotButton.SlotType.Player)
                    InventoryManager.Instance.RemoveItemCompletely(index);
                else
                    activeChest.RemoveItemCompletely(index);
            }

            RefreshUI();
            return;
        }

        // Place one into empty
        if (clicked == null)
        {
            var one = new InventorySlot { item = heldSlot.item, count = 1 };

            if (type == SlotButton.SlotType.Player)
                InventoryManager.Instance.PlaceItemAt(one, index);
            else
                activeChest.PlaceItemAt(one, index);

            heldSlot.count--;
            if (heldSlot.count <= 0)
                ClearHeldItem();

            RefreshUI();
            return;
        }

        // Place one into same stack
        if (clicked.item == heldSlot.item && clicked.count < clicked.item.MaxStack)
        {
            clicked.count++;
            heldSlot.count--;

            if (heldSlot.count <= 0) ClearHeldItem();

            RefreshUI();
            return;
        }
    }

    // Escape closes chest
    public void OnUICancel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            CloseChest();
    }
}
