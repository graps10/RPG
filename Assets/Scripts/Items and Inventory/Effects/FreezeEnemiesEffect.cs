using Enemies.Base;
using Managers;
using Stats;
using UnityEngine;

namespace Items_and_Inventory.Effects
{
    [CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Data/Item Effect/Freeze enemies", order = 0)]
    public class FreezeEnemiesEffect : ItemEffect
    {
        private const float Low_Health_Threshold = 0.1f; // 10% of max health
        private const float Freeze_Radius = 2f; // freeze effect range
        
        [SerializeField] private float duration;

        public override void ExecuteEffect(Transform transform)
        {
            PlayerStats playerStats = PlayerManager.Instance.PlayerGameObject.GetComponent<PlayerStats>();

            if(playerStats.CurrentHealth > playerStats.GetMaxHealthValue() * Low_Health_Threshold) return;

            if(!Inventory.Instance.CanUseArmor()) return;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Freeze_Radius);

            foreach(var hit in colliders)
                hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
        }
    }
}
