using UnityEngine;

public class Lake : MonoBehaviour, IInteractable
{
    public string InteractionText => "Bathe";
    public bool StaysInteractable => true;

    private NeedsManager needsManager;

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
        needsManager.CleanSelf(100f);
        Debug.Log("Bathed.");
    }
}
