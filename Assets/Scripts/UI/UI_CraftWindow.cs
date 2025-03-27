using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;

    [SerializeField] private Image[] materialImage;

    [SerializeField] private Button craftButton;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0; i < _data.GetCraftingMaterials().Count; i++)
        {
            if(_data.GetCraftingMaterials().Count > materialImage.Length)
            {
                Debug.LogWarning("You have more materials amount then you have material slots in craft window");
            }

            materialImage[i].sprite = _data.GetCraftingMaterials()[i].data.icon;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();
            materialImage[i].color = Color.white;

            materialSlotText.text = _data.GetCraftingMaterials()[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        
        }

        itemIcon.sprite = _data.icon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.GetCraftingMaterials()));
    }
}
