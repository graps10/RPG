using Items_and_Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI_Elements
{
    public class EquipmentSlot : ItemSlot
    {
        [SerializeField] private EquipmentType slotType;

        private void OnValidate() 
        {
            gameObject.name = "Equipment slot - " + slotType;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if(item == null) return;
        
            Inventory.Instance.UnequipItem(item.GetData() as ItemData_Equipment);
            Inventory.Instance.AddItem(item.GetData() as ItemData_Equipment);
        
            ui.GetItemTooltip().HideToolTip();
        
            CleanUpSlot();
        }
        
        public EquipmentType GetEquipmentType() => slotType;
    }
}
