using UnityEngine;

public class StarterBed : MonoBehaviour, IInteractable
{
    public string InteractionText => "Sleep";
    private NeedsManager needsManager;
    public bool StaysInteractable => true;


    void Start()
    {
        if (needsManager == null)
        {
            needsManager = FindFirstObjectByType<NeedsManager>();
        }
    }

    public void Interact()
    {
        SimulationManager sim = SimulationManager.Instance;

        // Check if it's night
        if (!sim.IsNight())
        {
            Debug.Log("You can only sleep at night.");
            return;
        }

        // Begin fast forward to morning
        sim.SleepFastForward(6f);  // Wake up at 6 AM

        // Run needs logic
        needsManager.Sleep();

        Debug.Log("Sleeping until morning...");
    }
}
