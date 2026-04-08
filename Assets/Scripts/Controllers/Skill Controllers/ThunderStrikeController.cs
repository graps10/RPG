using Core.ObjectPool;
using Managers;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class ThunderStrikeController : PooledObject
    {
        protected virtual void OnTriggerEnter2D(Collider2D collision) 
        {
            PlayerStats playerStats = PlayerManager.Instance.PlayerGameObject.Stats as PlayerStats;
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();

            if (enemyTarget != null && playerStats != null)
                playerStats.DoMagicalDamage(enemyTarget);
        }
    }
}
