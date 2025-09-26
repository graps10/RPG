using Items_and_Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI_Elements
{
    public class ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Image itemImage;
        [SerializeField] protected TextMeshProUGUI itemText;
        [SerializeField] protected InventoryItem item;

        protected UI ui;

        protected virtual void Start() 
        {
            ui = GetComponentInParent<UI>();
        }

        public void UpdateSlot(InventoryItem newItem)
        {
            item = newItem;

            itemImage.color = Color.white;

            if (item != null)
            {
                itemImage.sprite = item.GetData().Icon;

                if (item.GetStackSize() > 1)
                    itemText.text = item.GetStackSize().ToString();
                else
                    ClearItemText();
            }
        }

        public void CleanUpSlot()
        {
            item = null;

            itemImage.sprite = null;
            itemImage.color = Color.clear;

            ClearItemText();
        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if(item == null) return;

            if(Input.GetKey(KeyCode.LeftControl))
            {
                Inventory.Instance.RemoveItem(item.GetData());
                ui.GetItemTooltip().HideToolTip();
                return;
            }

            if(item.GetData().ItemType == ItemType.Equipment)
                Inventory.Instance.EquipItem(item.GetData());

            ui.GetItemTooltip().HideToolTip();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(item == null) return;

            ui.GetItemTooltip().ShowToolTip(item.GetData() as ItemData_Equipment);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(item == null) return;

            ui.GetItemTooltip().HideToolTip();
        }
        
        private void ClearItemText()
        {
            itemText.text = string.Empty;
        }
    }
}
