using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's Drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;
    
    private List<InventoryItem> equipment => inventory.GetEquipmentList();
    private List<InventoryItem> stash => inventory.GetStashList();
    
    private List<InventoryItem> itemsToUnequipment = new List<InventoryItem>();
    private List<InventoryItem> materialsToLoose = new List<InventoryItem>();

    Inventory inventory => Inventory.instance;

    public override void GenerateDrop()
    {
        GenerateEquipmentToDrop();
        GenerateMaterialsToDrop();

        itemsToUnequipment.Clear();
        materialsToLoose.Clear();
    }

    private void GenerateEquipmentToDrop()
    {
        foreach (InventoryItem item in equipment)
        {
            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequipment.Add(item);
            }
        }

        for (int i = 0; i < itemsToUnequipment.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequipment[i].data as ItemData_Equipment);
        }
    }

    private void GenerateMaterialsToDrop()
    {
        foreach (InventoryItem item in stash)
        {
            if (Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialsToLoose.Add(item);
            }
        }

        for (int i = 0; i < materialsToLoose.Count; i++)
        {
            inventory.RemoveItem(materialsToLoose[i].data);
        }
    }
}
