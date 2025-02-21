using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private int defaultFontSize = 32;

    public void ShowToolTip(ItemData_Equipment _item)
    {
        if(_item == null) return;

        itemNameText.text = _item.itemName;
        itemTypeText.text = _item.equipmentType.ToString();
        itemDescription.text = _item.GetDescription();

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
