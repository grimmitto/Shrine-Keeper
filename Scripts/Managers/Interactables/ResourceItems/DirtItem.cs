using UnityEngine;

public class DirtItem : BaseResourceItem
{
    public static readonly DirtItem Instance = new DirtItem();
    public override string ItemName => "Dirt";
}
