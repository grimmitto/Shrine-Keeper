using UnityEngine;
using UnityEngine.EventSystems;

public class SlotButton : MonoBehaviour, IPointerClickHandler
{
    public enum SlotType { Player, Chest }
    public SlotType slotType;
    public int index;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ChestUI.Instance.OnLeftClickSlot(slotType, index);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ChestUI.Instance.OnRightClickSlot(slotType, index);
        }
    }
}
