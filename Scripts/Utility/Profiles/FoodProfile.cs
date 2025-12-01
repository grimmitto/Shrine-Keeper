using UnityEngine;

[CreateAssetMenu(menuName = "Items/Food Item")]
public class FoodItem : ScriptableObject
{
    [Header("Basic Info")]
    public string foodName;
    [TextArea] public string description;
    public float nutritionalValue;
    public Sprite icon;

    [Header("Models")]
    public GameObject fullModel;          // Complete untouched dish
    public GameObject halfEatenModel;     // Partially eaten or mid-state
    public GameObject emptyModel;         // Finished/empty (optional)

  
}


