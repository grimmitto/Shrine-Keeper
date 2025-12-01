using UnityEngine;

public class Stones : MonoBehaviour, IDebrisInteractable
{
    public int cleaningValue => 1;
    public string resourceType => "stone";

    public string InteractionText => "Collect Stone";
    public bool StaysInteractable => false;


    public void Interact()
    {
        var prog = SimulationManager.Instance.progression;
        int current = (int)prog.variablesState["maintenance_state"];
        current = Mathf.Min(current + cleaningValue, 100);
        prog.variablesState["maintenance_state"] = current;
        ShrineManager.Instance.reputation = current;  // or a scaled version
        ShrineManager.Instance.UpdateUI();


        MaintenanceManager.Instance.SetMaintenance(current);

        InventoryManager.Instance.AddItem(StoneItem.Instance);


        Destroy(gameObject);
    }
}
