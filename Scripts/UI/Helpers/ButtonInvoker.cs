using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonInvoker : MonoBehaviour
{
    [HideInInspector] public Button targetButton;

    void Awake()
    {
        if (targetButton == null)
            targetButton = GetComponent<Button>();
    }

    public void PressButton()
    {
        // Only continue if THIS button is the one under the pointer
        if (EventSystem.current.currentSelectedGameObject != gameObject)
            return;

        targetButton.onClick.Invoke();
    }
}
