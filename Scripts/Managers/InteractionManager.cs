using UnityEngine;
using TMPro;


public class InteractionManager : MonoBehaviour
{
    public GameObject interactionUI;
    public TMP_Text interactionUIText;
    private IInteractable currentInteractable;

    public static InteractionManager Instance;
    void Awake() => Instance = this;

    void Start()
    {
        interactionUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (interactable is IStorable storable)
            {
                if (InventoryManager.Instance.items.Contains(storable))
                    return; // don't show text for items already held
            }

            currentInteractable = interactable;
            interactionUIText.text = $"Press E to {interactable.InteractionText}";
            interactionUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == currentInteractable)
        {
            currentInteractable = null;
            interactionUIText.text = string.Empty;
            interactionUI.SetActive(false);
        }
    }

    public void ReactivateUI()
    {
        if (currentInteractable != null)
        {
            interactionUIText.text = $"Press E to {currentInteractable.InteractionText}";
            interactionUI.SetActive(true);
        }
    }


    public void OnInteract()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact();
        interactionUI.SetActive(false);

        // Only clear if this interactable does NOT stay active
        if (!currentInteractable.StaysInteractable)
            currentInteractable = null;

    }

}
