using UnityEngine;

public class StoneItem : BaseResourceItem
{
    // 🔥 Singleton instance so inventory stacking works
    public static readonly StoneItem Instance = new StoneItem();

      public override Sprite InventorySprite
    {
        get
        {
            return InventoryManager.Instance.inventoryUI.stoneSprite;
        }
    }



    public override string ItemName => "Stone";

}
