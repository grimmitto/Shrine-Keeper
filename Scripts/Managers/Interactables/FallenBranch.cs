using UnityEngine;

public class FallenBranch : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int cleaningValue = 1;   // How much this item cleans when picked up

    public string InteractionText => "Pick Up Branch";
    public bool StaysInteractable => false;

    public void Interact()
    {
        // Add maintenance through the unified manager
        MaintenanceManager.Instance.AddMaintenance(cleaningValue);

        // Give the player wood
        InventoryManager.Instance.AddItem(WoodItem.Instance);


        // Remove the branch from the world
        Destroy(gameObject);
    }
}
