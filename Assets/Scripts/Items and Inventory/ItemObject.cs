using Components.Audio;
using Core.ObjectPool;
using Core.ObjectPool.Configs;
using Core.ObjectPool.Configs.Inventory;
using Managers;
using UnityEngine;

namespace Items_and_Inventory
{
    public class ItemObject : PooledObject
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private ItemData itemData;
        [SerializeField] private Vector2 velocity;
        
        private ItemDropPoolConfig _config;

        public void SetupItem(ItemData itemData, Vector2 velocity,  ItemDropPoolConfig config)
        {
            this.itemData = itemData;
            rb.velocity = velocity;
            _config = config;
            
            SetupVisual();
        }

        public override void ReturnToPool()
        {
            itemData = null;
            transform.position = Vector2.zero;
            base.ReturnToPool();
        }

        public void PickupItem()
        {
            if (!Inventory.Instance.CanAddItem() && itemData.ItemType == ItemType.Equipment)
            {
                rb.velocity = new Vector2(0, _config.ItemBounceForce);
                PlayerManager.Instance.PlayerGameObject.Fx.CreatePopUpText("Inventory is full");
                return;
            }

            AudioManager.Instance.PlaySFX(SFXEnum.ItemPickup, transform);
            Inventory.Instance.AddItem(itemData);

            ReturnToPool();
        }

        private void SetupVisual()
        {
            if (itemData == null) return;

            GetComponent<SpriteRenderer>().sprite = itemData.Icon;
            gameObject.name = "Item object - " + itemData.ItemName;
        }
    }
}
