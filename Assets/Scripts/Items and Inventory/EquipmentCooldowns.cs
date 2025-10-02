using Managers;
using UnityEngine;

namespace Items_and_Inventory
{
    public class EquipmentCooldowns
    {
        private Inventory _inventory;
        
        private float _flaskCooldown;
        private float _armorCooldown;
        
        private float _lastTimeUsedFlask;
        private float _lastTimeUsedArmor;

        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
        }

        public void TryUseFlask()
        {
            ItemData_Equipment currentFlask = _inventory.GetEquippedItem(EquipmentType.Flask);

            if (currentFlask == null)
            {
                PlayerManager.Instance.PlayerGameObject.Fx.CreatePopUpText("Empty flask slot");
                return;
            }
            
            // RemoveUsedFlask
            
            if (Time.time > _lastTimeUsedFlask + _flaskCooldown)
            {
                currentFlask.Effect();
                StartFlaskCooldown(currentFlask.GetItemCooldown());
            }
            else
                PlayerManager.Instance.PlayerGameObject.Fx.CreatePopUpText("Cooldown");
        }

        public bool CanUseArmor()
        {
            ItemData_Equipment currentArmor = _inventory.GetEquippedItem(EquipmentType.Armor);

            if (Time.time > _lastTimeUsedArmor + _armorCooldown)
            {
                StartArmorCooldown(currentArmor.GetItemCooldown());
                return true;
            }

            Debug.Log("Armor on cooldown");
            return false;
        }

        public void StartFlaskCooldown(float cooldown)
        {
            _flaskCooldown = cooldown;
            _lastTimeUsedFlask = Time.time;
        }

        public void StartArmorCooldown(float cooldown)
        {
            _armorCooldown = cooldown;
            _lastTimeUsedArmor = Time.time;
        }

        public float GetFlaskCooldown() => _flaskCooldown;
        public float GetFlaskCooldownRemaining() => Mathf.Max(0, (_lastTimeUsedFlask + _flaskCooldown) - Time.time);
        public float GetArmorCooldownRemaining() => Mathf.Max(0, (_lastTimeUsedArmor + _armorCooldown) - Time.time);
    }
}