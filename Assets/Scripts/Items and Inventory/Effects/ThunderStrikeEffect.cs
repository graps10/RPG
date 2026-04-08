using Core.ObjectPool.Configs.FX;
using UnityEngine;
using PoolManager = Core.ObjectPool.PoolManager;

namespace Items_and_Inventory.Effects
{
    [CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item Effect/Thunder strike", order = 0)]
    public class ThunderStrikeEffect : ItemEffect
    {
        [SerializeField] private ThunderStrikePoolConfig thunderStrikeConfig;
        public override void ExecuteEffect(Transform enemyPosition)
        {
            GameObject newThunderStrike = PoolManager.Instance.Spawn(thunderStrikeConfig.Prefab, 
                enemyPosition.position, Quaternion.identity);
            
            PoolManager.Instance.ReturnWithDelay(newThunderStrike, thunderStrikeConfig.ReturnToPoolDelay);
        }
    }
}
