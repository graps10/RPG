using UnityEngine;

namespace Core.ObjectPool.Configs.Inventory
{
    [CreateAssetMenu(fileName = "ItemsDropPoolConfig", menuName = "ObjectPool/Configs/Inventory/Item Drop")]
    public class ItemDropPoolConfig : BasePoolConfig
    {
        [Header("Item Drop Specific Settings")]
        [SerializeField] private float itemBounceForce = 7f;
        [SerializeField] private Vector2 dropVelocityXRange = new(-5f, 5f);
        [SerializeField] private Vector2 dropVelocityYRange = new(15f, 20f);
        
        public float ItemBounceForce =>  itemBounceForce;
        public Vector2 DropVelocityXRange =>  dropVelocityXRange;
        public Vector2 DropVelocityYRange =>  dropVelocityYRange;
    }
}