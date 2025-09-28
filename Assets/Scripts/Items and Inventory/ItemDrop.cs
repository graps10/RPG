using System.Collections.Generic;
using Core.ObjectPool;
using Managers;
using UnityEngine;

namespace Items_and_Inventory
{
    public class ItemDrop : MonoBehaviour
    {
        private static readonly Vector2 dropVelocityXRange = new(-5f, 5f);
        private static readonly Vector2 dropVelocityYRange = new(15f, 20f);
    
        [SerializeField] private int maxItemsToDrop;
        [SerializeField] private ItemData[] itemPool;
        [SerializeField] private GameObject dropPrefab;
    
        private List<ItemData> _possibleDrop = new();

        public virtual void GenerateDrop()
        {
            if (itemPool.Length == 0)
            {
                Debug.Log("Item Pool is empty. Enemy cannot drop items.");
                return;
            }

            foreach (ItemData item in itemPool)
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
            GameObject newDrop = PoolManager.Instance.Spawn(PoolNames.Drop, transform.position, Quaternion.identity, dropPrefab);

            Vector2 randomVelocity = new Vector2(
                Random.Range(dropVelocityXRange.x, dropVelocityXRange.y),
                Random.Range(dropVelocityYRange.x, dropVelocityYRange.y)
            );

            newDrop.GetComponent<ItemObject>().SetupItem(itemData, randomVelocity);
        }
    }
}
