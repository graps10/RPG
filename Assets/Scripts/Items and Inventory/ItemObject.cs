using Core.ObjectPool;
using Managers;
using UnityEngine;

namespace Items_and_Inventory
{
    public class ItemObject : MonoBehaviour, IPooledObject
    {
        private const float Item_Bounce_Force = 7f;
            
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private ItemData itemData;
        [SerializeField] private Vector2 velocity;

        public void SetupItem(ItemData itemData, Vector2 velocity)
        {
            this.itemData = itemData;
            rb.velocity = velocity;

            SetupVisual();
        }

        public void OnReturnToPool()
        {
            transform.position = Vector2.zero;
            itemData = null;
            gameObject.SetActive(false);
        }

        public void PickupItem()
        {
            if (!Inventory.Instance.CanAddItem() && itemData.ItemType == ItemType.Equipment)
            {
                rb.velocity = new Vector2(0, Item_Bounce_Force);
                PlayerManager.Instance.PlayerGameObject.Fx.CreatePopUpText("Inventory is full");
                return;
            }

            AudioManager.Instance.PlaySFX(16, transform);
            Inventory.Instance.AddItem(itemData);

            PoolManager.Instance.Return("drop", gameObject);
        }

        private void SetupVisual()
        {
            if (itemData == null) return;

            GetComponent<SpriteRenderer>().sprite = itemData.Icon;
            gameObject.name = "Item object - " + itemData.ItemName;
        }
    }
}
