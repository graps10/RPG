using System.Collections.Generic;
using Core.Save_and_Load;
using UI_Elements;
using UnityEditor;
using UnityEngine;

namespace Items_and_Inventory
{
    public class Inventory : MonoBehaviour, ISaveManager
    {
        private const string Item_Database_Path = "Assets/Data/Items";
        
        public static Inventory Instance;

        [SerializeField] private InventoryUI ui = new();
        
        [SerializeField] private List<ItemData> startingItems;
        [SerializeField] private List<InventoryItem> equipment = new();
        [SerializeField] private List<InventoryItem> inventory = new();
        [SerializeField] private List<InventoryItem> stash = new();
        
        [Header("Data Base")]
        [SerializeField] private List<ItemData> itemDataBase;
        [SerializeField] private List<InventoryItem> loadedItems;
        [SerializeField] private List<ItemData_Equipment> loadedEquipment;
        
        private Dictionary<ItemData_Equipment, InventoryItem> _equipmentDictionary = new();
        private Dictionary<ItemData, InventoryItem> _inventoryDictionary = new();
        private Dictionary<ItemData, InventoryItem> _stashDictionary = new();
        
        private EquipmentCooldowns _cooldowns = new();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            _cooldowns.Initialize(this);
            ui.Initialize(inventory, stash, _equipmentDictionary);
           
            AddStartingItems();
        }

       private void AddStartingItems()
        {
            foreach (ItemData_Equipment item in loadedEquipment)
                EquipItem(item);

            if (loadedItems.Count > 0)
            {
                foreach (InventoryItem item in loadedItems)
                    for (int i = 0; i < item.GetStackSize(); i++)
                        AddItem(item.GetData());
                return;
            }

            for (int i = 0; i < startingItems.Count; i++)
                if (startingItems[i] != null)
                    AddItem(startingItems[i]);
        }

        public bool CanAddItem()
        {
            if (inventory.Count >= ui.GetInventorySlots().Length)
            {
                Debug.Log("No more space");
                return false;
            }

            return true;
        }

        public bool CanCraft(ItemData_Equipment itemToCraft, List<InventoryItem> requiredMaterials)
        {
            foreach (var requiredItem in requiredMaterials)
            {
                if (_stashDictionary.TryGetValue(requiredItem.GetData(), out InventoryItem stashItem))
                {
                    if (stashItem.GetStackSize() < requiredItem.GetStackSize())
                    {
                        Debug.Log("Not enough materials: " + requiredItem.GetData().name);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("Materials are not found in stash: " + requiredItem.GetData().name);
                    return false;
                }
            }

            foreach (var requiredMaterial in requiredMaterials)
            {
                for (int i = 0; i < requiredMaterial.GetStackSize(); i++)
                {
                    RemoveItem(requiredMaterial.GetData());
                }
            }

            AddItem(itemToCraft);
            Debug.Log("Craft is successful");
            return true;
        }

        public void EquipItem(ItemData _item)
        {
            ItemData_Equipment newEquipment = _item as ItemData_Equipment;
            InventoryItem newItem = new InventoryItem(newEquipment);

            ItemData_Equipment oldEquipment = null;

            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in _equipmentDictionary)
            {
                if (item.Key.equipmentType == newEquipment.equipmentType)
                    oldEquipment = item.Key;
            }

            if (oldEquipment != null)
            {
                UnequipItem(oldEquipment);
                AddItem(oldEquipment);
            }

            equipment.Add(newItem);
            _equipmentDictionary.Add(newEquipment, newItem);

            newEquipment.AddModifiers();

            RemoveItem(_item);

            ui.UpdateSlotUI();
        }

        public void UnequipItem(ItemData_Equipment itemToRemove)
        {
            if (itemToRemove == null) return;

            if (_equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
            {
                equipment.Remove(value);
                _equipmentDictionary.Remove(itemToRemove);
                itemToRemove.RemoveModifiers();
            }
        }

        public List<InventoryItem> GetEquipment() => equipment;
        public List<InventoryItem> GetStash() => stash;
        public ItemData_Equipment GetEquippedItem(EquipmentType type)
        {
            ItemData_Equipment equippedItem = null;

            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in _equipmentDictionary)
            {
                if (item.Key.equipmentType == type)
                    equippedItem = item.Key;
            }

            return equippedItem;
        }
        public EquipmentCooldowns GetEquipmentCooldowns() => _cooldowns;

        public void AddItem(ItemData item)
        {
            if (item == null) return;

            if (item.ItemType == ItemType.Equipment && CanAddItem())
                AddToInventory(item);
            else if (item.ItemType == ItemType.Material)
                AddToStash(item);

            ui.UpdateSlotUI();
        }

        public void RemoveItem(ItemData item)
        {
            if (_inventoryDictionary.TryGetValue(item, out InventoryItem inventoryValue))
            {
                if (inventoryValue.GetStackSize() <= 1)
                {
                    inventory.Remove(inventoryValue);
                    _inventoryDictionary.Remove(item);
                }
                else
                    inventoryValue.RemoveStack();
            }

            if (_stashDictionary.TryGetValue(item, out InventoryItem stashValue))
            {
                if (stashValue.GetStackSize() <= 1)
                {
                    stash.Remove(stashValue);
                    _stashDictionary.Remove(item);
                }
                else
                    stashValue.RemoveStack();
            }

            ui.UpdateSlotUI();
        }

        public void LoadData(GameData data)
        {
            foreach (KeyValuePair<string, int> pair in data.GetInventory())
            {
                foreach (var item in itemDataBase)
                {
                    if (item != null && item.ItemID == pair.Key)
                    {
                        InventoryItem itemToLoad = new InventoryItem(item);
                        itemToLoad.SetStackSize(pair.Value);

                        loadedItems.Add(itemToLoad);
                    }
                }
            }

            foreach (string loadedItemID in data.GetEquipmentList())
            {
                foreach (var item in itemDataBase)
                {
                    if (item != null && loadedItemID == item.ItemID)
                    {
                        loadedEquipment.Add(item as ItemData_Equipment);
                    }
                }
            }
        }

        public void SaveData(ref GameData data)
        {
            data.GetInventory().Clear();
            data.GetEquipmentList().Clear();

            foreach (KeyValuePair<ItemData, InventoryItem> pair in _inventoryDictionary)
                data.GetInventory().Add(pair.Key.ItemID, pair.Value.GetStackSize());

            foreach (KeyValuePair<ItemData, InventoryItem> pair in _stashDictionary)
                data.GetInventory().Add(pair.Key.ItemID, pair.Value.GetStackSize());

            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in _equipmentDictionary)
                data.GetEquipmentList().Add(pair.Key.ItemID);
        }

        private void AddToStash(ItemData item)
        {
            if (_stashDictionary.TryGetValue(item, out InventoryItem value))
                value.AddStack();
            else
            {
                InventoryItem newItem = new InventoryItem(item);
                stash.Add(newItem);
                _stashDictionary.Add(item, newItem);
            }
        }

        private void AddToInventory(ItemData item)
        {
            if (_inventoryDictionary.TryGetValue(item, out InventoryItem value))
                value.AddStack();
            else
            {
                InventoryItem newItem = new InventoryItem(item);
                inventory.Add(newItem);
                _inventoryDictionary.Add(item, newItem);
            }
        }

#if UNITY_EDITOR

        [ContextMenu("Fill up item data base")]
        private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDatabase());

        private static List<ItemData> GetItemDatabase()
        {
            List<ItemData> itemDataBase = new List<ItemData>();
            string[] assetNames = AssetDatabase.FindAssets("", new[] { Item_Database_Path });

            foreach (string SOName in assetNames)
            {
                var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
                var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
                itemDataBase.Add(itemData);
            }

            return itemDataBase;
        }

#endif
    }
}
