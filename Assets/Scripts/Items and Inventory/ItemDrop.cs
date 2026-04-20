using System.Collections.Generic;
using Core.ObjectPool.Configs.Inventory;
using UnityEngine;
using PoolManager = Core.ObjectPool.PoolManager;

namespace Items_and_Inventory
{
    public class ItemDrop : MonoBehaviour
    {
        [SerializeField] protected ItemDropPoolConfig itemDropConfig;
    
        protected readonly List<ItemData> _possibleDrop = new();

        public virtual void GenerateDrop() { }

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
