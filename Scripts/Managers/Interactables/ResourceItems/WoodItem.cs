using UnityEngine;

public class WoodItem : BaseResourceItem
{
    // 🔥 Singleton instance
    public static readonly WoodItem Instance = new WoodItem();

    private Sprite _icon;

   public override Sprite InventorySprite
    {
        get
        {
            return InventoryManager.Instance.inventoryUI.woodSprite;
        }
    }


    public override string ItemName => "Wood";

}
