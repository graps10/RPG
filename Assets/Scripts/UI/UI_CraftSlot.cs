using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{

    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if(_data == null) return;
        
        item.data = _data;

        itemImage.sprite = _data.icon;
        itemText.text = _data.itemName;

        if(itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * 0.7f;
        else
            itemText.fontSize = 24;

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}
