using UnityEngine;

public class DirtPile : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int cleaningValue = 1;   // How much this reduces dirt when cleaned

    public string InteractionText => "Clean Dirt";
    public bool StaysInteractable => false;

    public void Interact()
    {
        // Add maintenance through the unified method
        MaintenanceManager.Instance.AddMaintenance(cleaningValue);

        // Optionally drop dirt item (only if you want dirt as an inventory item)
        InventoryManager.Instance.AddItem(DirtItem.Instance);


        // Remove the dirt pile
        Destroy(gameObject);
    }
}
