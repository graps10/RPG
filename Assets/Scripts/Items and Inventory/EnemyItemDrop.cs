using UnityEngine;

namespace Items_and_Inventory
{
    public class EnemyItemDrop : ItemDrop
    {
        [SerializeField] private int maxItemsToDrop;
        [SerializeField] private ItemData[] itemList;

        public virtual void GenerateDrop()
        {
            if (itemList.Length == 0)
            {
                Debug.Log("Item Pool is empty. Enemy cannot drop items.");
                return;
            }

            foreach (ItemData item in itemList)
            {
                if (item != null && Random.Range(0, 100) < item.DropChance)
                    _possibleDrop.Add(item);
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