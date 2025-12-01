using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotButton : MonoBehaviour, IPointerClickHandler
{
    public int slotNumber;  // 1, 2, or 3
    public MainMenuSceneHelper menu; // reference to update UI after deletion

    public void OnPointerClick(PointerEventData eventData)
    {
        // LEFT CLICK → play
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            menu.PlaySlot(slotNumber);
        }

        // RIGHT CLICK → delete save
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DeleteSlot();
        }
    }

    public void PlaySlot()
    {
        menu.PlaySlot(slotNumber);
    }

    public void DeleteSlot()
    {
        SaveUtility.DeleteSave(slotNumber);
        menu.RefreshSlots();
    }
}
