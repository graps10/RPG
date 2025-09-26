using System.Collections.Generic;
using UnityEngine;

namespace Items_and_Inventory
{
    public class PlayerItemDrop : ItemDrop
    {
        [Header("Player's Drop")]
        [SerializeField] private float chanceToLooseItems;
        [SerializeField] private float chanceToLooseMaterials;
        
        private List<InventoryItem> _itemsToUnequipment = new();
        private List<InventoryItem> _materialsToLoose = new();

        public override void GenerateDrop()
        {
            GenerateEquipmentToDrop();
            GenerateMaterialsToDrop();

            _itemsToUnequipment.Clear();
            _materialsToLoose.Clear();
        }

        private void GenerateEquipmentToDrop()
        {
            foreach (InventoryItem item in Inventory.Instance.GetEquipment())
            {
                if (Random.Range(0, 100) <= chanceToLooseItems)
                {
                    DropItem(item.GetData());
                    _itemsToUnequipment.Add(item);
                }
            }

            for (int i = 0; i < _itemsToUnequipment.Count; i++)
                Inventory.Instance.UnequipItem(_itemsToUnequipment[i].GetData() as ItemData_Equipment);
        }

        private void GenerateMaterialsToDrop()
        {
            foreach (InventoryItem item in Inventory.Instance.GetStash())
            {
                if (Random.Range(0, 100) <= chanceToLooseMaterials)
                {
                    DropItem(item.GetData());
                    _materialsToLoose.Add(item);
                }
            }

            for (int i = 0; i < _materialsToLoose.Count; i++)
                Inventory.Instance.RemoveItem(_materialsToLoose[i].GetData());
        }
    }
}
