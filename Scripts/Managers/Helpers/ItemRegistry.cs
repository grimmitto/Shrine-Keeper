using UnityEngine;
using System.Collections.Generic;

public class ItemRegistryInitializer : MonoBehaviour
{
    public List<GameObject> storablePrefabs;  // drag all item prefabs here

    private void Awake()
    {
        // Register code-defined item singletons
        ItemRegistry.Register(StoneItem.Instance);
        ItemRegistry.Register(WoodItem.Instance);
        ItemRegistry.Register(DirtItem.Instance);

        // Existing prefab-based registration
        foreach (var prefab in storablePrefabs)
        {
            var storable = prefab.GetComponent<IStorable>();
            if (storable != null)
                ItemRegistry.Register(storable);
        }
    }



}




