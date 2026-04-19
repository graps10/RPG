using System.Collections.Generic;
using Items_and_Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI_Elements
{
    public class CraftList : MonoBehaviour , IPointerDownHandler
    {
        [SerializeField] private Transform craftSlotParent;
        [SerializeField] private GameObject craftSlotPrefab;
        [SerializeField] private List<ItemData_Equipment> craftEquipment;
        
        private List<CraftSlot> _pooledSlots = new();

        private void Start()
        {
            transform.parent.GetChild(0).GetComponent<CraftList>().SetupCraftList();
            SetupDefaultCraftWindow();
        }

        public void SetupCraftList()
        {
            for (int i = 0; i < craftSlotParent.childCount; i++)
            {
                craftSlotParent.GetChild(i).gameObject.SetActive(false);
            }
            
            for (int i = 0; i < craftEquipment.Count; i++)
            {
                CraftSlot currentSlot = null;
                
                if (i < _pooledSlots.Count)
                {
                    currentSlot = _pooledSlots[i];
                    currentSlot.gameObject.SetActive(true);
                }
                else
                {
                    GameObject newSlotObj = Instantiate(craftSlotPrefab, craftSlotParent);
                    currentSlot = newSlotObj.GetComponent<CraftSlot>();
                    _pooledSlots.Add(currentSlot);
                }
                
                currentSlot.SetupCraftSlot(craftEquipment[i]);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetupCraftList();
        }

        public void SetupDefaultCraftWindow()
        {
            if (craftEquipment != null && craftEquipment.Count > 0 && craftEquipment[0] != null)
                GetComponentInParent<UI>().GetCraftWindow().SetupCraftWindow(craftEquipment[0]);
        }
    }
}
