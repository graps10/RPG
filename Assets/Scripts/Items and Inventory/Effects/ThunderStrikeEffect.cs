using Managers;
using UnityEngine;

namespace Items_and_Inventory.Effects
{
    [CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item Effect/Thunder strike", order = 0)]
    public class ThunderStrikeEffect : ItemEffect
    {
        private const float Return_To_Pool_Delay = 1f;
        
        [SerializeField] private GameObject thunderStrikePrefab;
        public override void ExecuteEffect(Transform enemyPosition)
        {
            GameObject newThunderStrike = PoolManager.Instance.Spawn("fx", 
                enemyPosition.position, Quaternion.identity, thunderStrikePrefab);
            
            PoolManager.Instance.Return("fx", newThunderStrike, Return_To_Pool_Delay);
        }
    }
}
