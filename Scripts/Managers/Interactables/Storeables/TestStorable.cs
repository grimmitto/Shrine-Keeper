using UnityEngine;
public class TestStorable : MonoBehaviour, IStorable, IInteractable
{
    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private int maxStack = 1;
    [SerializeField] private GameObject worldPrefab;

    public Sprite InventorySprite => inventorySprite;
    public int MaxStack => maxStack;
    public GameObject WorldPrefab => worldPrefab;
    public string ItemName => gameObject.name;
    public bool StaysInteractable => false;


    public string InteractionText => $"Pick up Food";

    public void Interact()
    {
        if (InventoryManager.Instance.AddItem(this))
        {
            // Hide or destroy the in-world object
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Inventory full");
        }
    }
}
