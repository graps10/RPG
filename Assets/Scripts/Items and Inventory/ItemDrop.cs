using System.Collections.Generic;
using Core.ObjectPool.Configs;
using Core.ObjectPool.Configs.Inventory;
using UnityEngine;
using PoolManager = Core.ObjectPool.PoolManager;

namespace Items_and_Inventory
{
    public class ItemDrop : MonoBehaviour
    {
        [SerializeField] private int maxItemsToDrop;
        [SerializeField] private ItemData[] itemList;
        [SerializeField] private ItemDropPoolConfig itemDropConfig;
    
        private readonly List<ItemData> _possibleDrop = new();

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

        protected void DropItem(ItemData itemData)
        {
            GameObject newDrop = PoolManager.Instance.Spawn(itemDropConfig.Prefab, transform.position, Quaternion.identity);

            Vector2 randomVelocity = new Vector2(
                Random.Range(itemDropConfig.DropVelocityXRange.x, itemDropConfig.DropVelocityXRange.y),
                Random.Range(itemDropConfig.DropVelocityYRange.x, itemDropConfig.DropVelocityYRange.y)
            );

            newDrop.GetComponent<ItemObject>().SetupItem(itemData, randomVelocity, itemDropConfig);
        }
    }
}
