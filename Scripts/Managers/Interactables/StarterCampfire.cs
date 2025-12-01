using UnityEngine;

public class StarterCampfire : MonoBehaviour, IInteractable
{
    public string InteractionText => "Start Fire";
    private NeedsManager needsManager;
    public bool StaysInteractable => true;


    void Start()
    {
        if (needsManager == null)
        {
            needsManager = FindFirstObjectByType<NeedsManager>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Interact()
    {
        
        Debug.Log("Start fire.");
    }
}
