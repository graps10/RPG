using System.Collections.Generic;
using Core.Save_and_Load;
using Managers;
using UI_Elements;
using UnityEditor;
using UnityEngine;

namespace Items_and_Inventory
{
    public class Inventory : MonoBehaviour, ISaveManager
    {
        private const string Item_Database_Path = "Assets/Data/Items";
        
        public static Inventory Instance;

        [SerializeField] private List<ItemData> startingItems;
        [SerializeField] private List<InventoryItem> equipment;
        [SerializeField] private List<InventoryItem> inventory;
        [SerializeField] private List<InventoryItem> stash;
        
        [Header("Inventory UI")]
        [SerializeField] private Transform inventorySlotParent;
        [SerializeField] private Transform stashSlotParent;
        [SerializeField] private Transform equipmentSlotParent;
        [SerializeField] private Transform statSlotParent;

        [Header("Data Base")]
        [SerializeField] private List<ItemData> itemDataBase;
        [SerializeField] private List<InventoryItem> loadedItems;
        [SerializeField] private List<ItemData_Equipment> loadedEquipment;
        
        private Dictionary<ItemData_Equipment, InventoryItem> _equipmentDictionary;
        private Dictionary<ItemData, InventoryItem> _inventoryDictionary;
        private Dictionary<ItemData, InventoryItem> stashDictionary;
        
        private ItemSlot[] _inventoryItemSlot;
        private ItemSlot[] _stashItemSlot;
        private EquipmentSlot[] _equipmentSlot;
        private StatSlot[] _statSlot;
        
        private float _flaskCooldown;
        private float _armorCooldown;
        
        private float _lastTimeUsedFlask;
        private float _lastTimeUsedArmor;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            inventory = new List<InventoryItem>();
            _inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

            stash = new List<InventoryItem>();
            stashDictionary = new Dictionary<ItemData, InventoryItem>();

            equipment = new List<InventoryItem>();
            _equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

            _inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<ItemSlot>();
            _stashItemSlot = stashSlotParent.GetComponentsInChildren<ItemSlot>();
            _equipmentSlot = equipmentSlotParent.GetComponentsInChildren<EquipmentSlot>();
            _statSlot = statSlotParent.GetComponentsInChildren<StatSlot>();

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
            if (inventory.Count >= _inventoryItemSlot.Length)
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
                if (stashDictionary.TryGetValue(requiredItem.GetData(), out InventoryItem stashItem))
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

            UpdateSlotUI();
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
        public ItemData_Equipment GetEquipment(EquipmentType type)
        {
            ItemData_Equipment equipedItem = null;

            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in _equipmentDictionary)
            {
                if (item.Key.equipmentType == type)
                    equipedItem = item.Key;
            }

            return equipedItem;
        }

        public void AddItem(ItemData item)
        {
            if (item == null) return;

            if (item.ItemType == ItemType.Equipment && CanAddItem())
                AddToInventory(item);
            else if (item.ItemType == ItemType.Material)
                AddToStash(item);

            UpdateSlotUI();
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

            if (stashDictionary.TryGetValue(item, out InventoryItem stashValue))
            {
                if (stashValue.GetStackSize() <= 1)
                {
                    stash.Remove(stashValue);
                    stashDictionary.Remove(item);
                }
                else
                    stashValue.RemoveStack();
            }

            UpdateSlotUI();
        }

        public bool CanUseArmor() // temporary code
        {
            ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

            if (Time.time > _lastTimeUsedArmor + _armorCooldown)
            {
                _armorCooldown = currentArmor.GetItemCooldown();
                _lastTimeUsedArmor = Time.time;
                return true;
            }

            Debug.Log("Armor on cooldown");
            return false;
        }

        public void UseFlask() // temporary code
        {
            ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

            if (currentFlask == null)
            {
                PlayerManager.Instance.PlayerGameObject.Fx.CreatePopUpText("Empty flask slot");
                return;
            }
            // RemoveUsedFlask(currentFlask);

            bool canUseFlask = Time.time > _lastTimeUsedFlask + _flaskCooldown;

            if (canUseFlask)
            {
                _flaskCooldown = currentFlask.GetItemCooldown();
                currentFlask.Effect(null);
                _lastTimeUsedFlask = Time.time;
            }
            else
                PlayerManager.Instance.PlayerGameObject.Fx.CreatePopUpText("Cooldown");
        }

        public float GetFlaskCooldown() => _flaskCooldown;

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

            foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
                data.GetInventory().Add(pair.Key.ItemID, pair.Value.GetStackSize());

            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in _equipmentDictionary)
                data.GetEquipmentList().Add(pair.Key.ItemID);
        }

        private void AddToStash(ItemData item)
        {
            if (stashDictionary.TryGetValue(item, out InventoryItem value))
                value.AddStack();
            else
            {
                InventoryItem newItem = new InventoryItem(item);
                stash.Add(newItem);
                stashDictionary.Add(item, newItem);
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

        private void UpdateSlotUI()
        {
            CleanSlotsUI();

            UpdateSlots();
            UpdateEquipmentSlotsUI();
        }

        private void UpdateSlots()
        {
            for (int i = 0; i < inventory.Count; i++)
                _inventoryItemSlot[i].UpdateSlot(inventory[i]);

            for (int i = 0; i < stash.Count; i++)
                _stashItemSlot[i].UpdateSlot(stash[i]);

            UpdateStatsUI();
        }

        private void CleanSlotsUI()
        {
            for (int i = 0; i < _inventoryItemSlot.Length; i++)
                _inventoryItemSlot[i].CleanUpSlot();

            for (int i = 0; i < _stashItemSlot.Length; i++)
                _stashItemSlot[i].CleanUpSlot();
        }

        public void UpdateStatsUI()
        {
            for (int i = 0; i < _statSlot.Length; i++)
            {
                _statSlot[i].UpdateStatValueUI();
            }
        }

        private void UpdateEquipmentSlotsUI()
        {
            for (int i = 0; i < _equipmentSlot.Length; i++)
            {
                foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in _equipmentDictionary)
                {
                    if (item.Key.equipmentType == _equipmentSlot[i].GetEquipmentType())
                        _equipmentSlot[i].UpdateSlot(item.Value);
                }
            }
        }

        private void RemoveUsedFlask(ItemData_Equipment currentFlask)
        {
            UnequipItem(currentFlask);
            RemoveItem(currentFlask);

            _equipmentSlot[3].CleanUpSlot();
        }

#if UNITY_EDITOR

        [ContextMenu("Fill up item data base")]
        private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDatabase());

        private List<ItemData> GetItemDatabase()
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
