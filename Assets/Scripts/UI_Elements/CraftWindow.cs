using Items_and_Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Elements
{
    public class CraftWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private Image itemIcon;

        [SerializeField] private Image[] materialImage;
        [SerializeField] private Button craftButton;

        public void SetupCraftWindow(ItemData_Equipment data)
        {
            craftButton.onClick.RemoveAllListeners();

            for (int i = 0; i < materialImage.Length; i++)
            {
                materialImage[i].color = Color.clear;
                materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
            }

            for (int i = 0; i < data.GetCraftingMaterials().Count; i++)
            {
                if(data.GetCraftingMaterials().Count > materialImage.Length)
                {
                    Debug.LogWarning("You have more materials amount then you have material slots in craft window");
                }

                materialImage[i].sprite = data.GetCraftingMaterials()[i].GetData().Icon;

                TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();
                materialImage[i].color = Color.white;

                materialSlotText.text = data.GetCraftingMaterials()[i].GetStackSize().ToString();
                materialSlotText.color = Color.white;
        
            }

            itemIcon.sprite = data.Icon;
            itemName.text = data.ItemName;
            itemDescription.text = data.GetDescription();

            craftButton.onClick.AddListener(() => Inventory.Instance.CanCraft(data, data.GetCraftingMaterials()));
        }
    }
}
