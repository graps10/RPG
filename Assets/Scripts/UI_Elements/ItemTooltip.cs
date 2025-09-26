using Items_and_Inventory;
using TMPro;
using UnityEngine;

namespace UI_Elements
{
    public class ItemTooltip : ToolTip
    {
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemTypeText;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private int defaultFontSize = 32;

        public void ShowToolTip(ItemData_Equipment item)
        {
            if(item == null) return;

            itemNameText.text = item.ItemName;
            itemTypeText.text = item.equipmentType.ToString();
            itemDescription.text = item.GetDescription();

            AdjustPosition();
            AdjustFontSize(itemNameText);
        
            gameObject.SetActive(true);
        }

        public void HideToolTip()
        {
            itemNameText.fontSize = defaultFontSize;
            gameObject.SetActive(false);
        }
    }
}
