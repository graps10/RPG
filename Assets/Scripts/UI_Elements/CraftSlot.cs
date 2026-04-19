using Items_and_Inventory;
using UnityEngine.EventSystems;

namespace UI_Elements
{
    public class CraftSlot : ItemSlot
    {
        public void SetupCraftSlot(ItemData_Equipment data)
        {
            if(data == null) return;
        
            item.SetData(data);

            itemImage.sprite = data.Icon;
            itemText.text = data.ItemName;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            ui.GetCraftWindow().SetupCraftWindow(item.GetData() as ItemData_Equipment);
        }
    }
}
