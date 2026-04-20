using UnityEngine;

namespace Items_and_Inventory
{
    [System.Serializable]
    public class EnemyDropData
    {
        public ItemData item;
        [Range(0, 100)] public float dropChance;
    }
    
    public class EnemyItemDrop : ItemDrop
    {
        [SerializeField] private int maxItemsToDrop;
        [SerializeField] private EnemyDropData[] dropList;

        public override void GenerateDrop()
        {
            if (dropList == null || dropList.Length == 0)
            {
                Debug.Log("Item Pool is empty. Enemy cannot drop items.");
                return;
            }
            
            foreach (EnemyDropData dropData in dropList)
            {
                if (dropData.item != null && Random.Range(0, 100) < dropData.dropChance)
                    _possibleDrop.Add(dropData.item);
            }

            for (int i = 0; i < maxItemsToDrop; i++)
            {
                if (_possibleDrop.Count > 0)
                {
                    int randomIndex = Random.Range(0, _possibleDrop.Count);
                    ItemData itemToDrop = _possibleDrop[randomIndex];

                    DropItem(itemToDrop);
                    _possibleDrop.Remove(itemToDrop);
                }
            }
        }
    }
}