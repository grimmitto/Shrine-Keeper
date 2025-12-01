using UnityEngine;

public abstract class BaseResourceItem : IStorable
{
    public abstract string ItemName { get; }
    public virtual int MaxStack => 20;
    public virtual Sprite InventorySprite => null;   // Assign later if you want icons
    public virtual GameObject WorldPrefab => null;   // For hand-mounted models
}
