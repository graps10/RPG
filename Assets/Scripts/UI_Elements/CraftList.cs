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

        private void Start()
        {
            transform.parent.GetChild(0).GetComponent<CraftList>().SetupCraftList();
            SetupDefaultCraftWindow();
        }

        public void SetupCraftList()
        {
            for (int i = 0; i < craftSlotParent.childCount; i++)
            {
                Destroy(craftSlotParent.GetChild(i).gameObject);
            }

            for (int i = 0; i < craftEquipment.Count; i++)
            {
                GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
                newSlot.GetComponent<CraftSlot>().SetupCraftSlot(craftEquipment[i]);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetupCraftList();
        }

        public void SetupDefaultCraftWindow()
        {
            if (craftEquipment[0] != null)
                GetComponentInParent<UI>().GetCraftWindow().SetupCraftWindow(craftEquipment[0]);
        }
    }
}
