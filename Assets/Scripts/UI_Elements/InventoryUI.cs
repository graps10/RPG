using System;
using System.Collections.Generic;
using Items_and_Inventory;
using UnityEngine;

namespace UI_Elements
{
    [Serializable]
    public class InventoryUI
    {
        [SerializeField] private Transform inventorySlotParent;
        [SerializeField] private Transform stashSlotParent;
        [SerializeField] private Transform equipmentSlotParent;
        [SerializeField] private Transform statSlotParent;
        
        private ItemSlot[] _inventoryItemSlot;
        private ItemSlot[] _stashItemSlot;
        private EquipmentSlot[] _equipmentSlot;
        private StatSlot[] _statSlot;

        private List<InventoryItem> _inventory;
        private List<InventoryItem> _stash;
        private Dictionary<ItemData_Equipment, InventoryItem> _equipmentDictionary;
        
        public void Initialize(List<InventoryItem> inventory, List<InventoryItem> stash, 
            Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary)
        {
            _inventory = inventory;
            _stash = stash;
            _equipmentDictionary = equipmentDictionary;

            _inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<ItemSlot>();
            _stashItemSlot = stashSlotParent.GetComponentsInChildren<ItemSlot>();
            _equipmentSlot = equipmentSlotParent.GetComponentsInChildren<EquipmentSlot>();
            _statSlot = statSlotParent.GetComponentsInChildren<StatSlot>();
        }
        
        public void UpdateSlotUI()
        {
            CleanSlotsUI();
            UpdateSlots();
            UpdateEquipmentSlotsUI();
        }

        private void UpdateSlots()
        {
            for (int i = 0; i < _inventory.Count; i++)
                _inventoryItemSlot[i].UpdateSlot(_inventory[i]);

            for (int i = 0; i < _stash.Count; i++)
                _stashItemSlot[i].UpdateSlot(_stash[i]);

            UpdateStatsUI();
        }

        private void CleanSlotsUI()
        {
            foreach (var slot in _inventoryItemSlot)
                slot.CleanUpSlot();

            foreach (var slot in _stashItemSlot)
                slot.CleanUpSlot();
        }

        public void UpdateStatsUI()
        {
            foreach (var slot in _statSlot)
                slot.UpdateStatValueUI();
        }

        private void UpdateEquipmentSlotsUI()
        {
            foreach (var slot in _equipmentSlot)
            {
                foreach (var equipment in _equipmentDictionary)
                {
                    if (equipment.Key.equipmentType == slot.GetEquipmentType())
                        slot.UpdateSlot(equipment.Value);
                }
            }
        }
        
        public ItemSlot[] GetInventorySlots() => _inventoryItemSlot;
        public EquipmentSlot[] GetEquipmentSlots() => _equipmentSlot;
    }
}