using System;
using UnityEngine;

namespace Items_and_Inventory
{
    [Serializable]
    public class InventoryItem
    {
        [SerializeField] private ItemData data;
        [SerializeField] private int stackSize;

        public InventoryItem(ItemData newItemData)
        {
            data = newItemData;
            AddStack();
        }

        public ItemData GetData() => data;
        public int GetStackSize() => stackSize;

        public void SetData(ItemData value) => data = value;
        public void SetStackSize(int value) => stackSize = value;
        public void AddStack() => stackSize++;

        public void RemoveStack() => stackSize--;
    }
}
