using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    void OnEnable() 
    {
        UpdateSlot(item);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ItemData_Equipment craftData = item.data as ItemData_Equipment;

        Inventory.instance.CanCraft(craftData, craftData.GetCraftingMaterials());
    }
}
