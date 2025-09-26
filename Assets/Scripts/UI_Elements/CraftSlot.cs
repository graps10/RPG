using Items_and_Inventory;
using UnityEngine.EventSystems;

namespace UI_Elements
{
    public class CraftSlot : ItemSlot
    {
        private const int Max_Item_Name_Length = 12;
        private const float Font_Size_Reduction_Factor = 0.7f;
        private const int Default_Font_Size = 24;
        
        public void SetupCraftSlot(ItemData_Equipment data)
        {
            if(data == null) return;
        
            item.SetData(data);

            itemImage.sprite = data.Icon;
            itemText.text = data.ItemName;

            if (itemText.text.Length > Max_Item_Name_Length)
                itemText.fontSize *= Font_Size_Reduction_Factor;
            else
                itemText.fontSize = Default_Font_Size;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            ui.GetCraftWindow().SetupCraftWindow(item.GetData() as ItemData_Equipment);
        }
    }
}
