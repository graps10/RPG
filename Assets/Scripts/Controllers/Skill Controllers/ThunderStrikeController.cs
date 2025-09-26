using Enemies.Base;
using Managers;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class ThunderStrikeController : MonoBehaviour
    {
        protected virtual void OnTriggerEnter2D(Collider2D collision) 
        {
            PlayerStats playerStats = PlayerManager.Instance.PlayerGameObject.GetComponent<PlayerStats>();

            if(collision.GetComponent<Enemy>() != null)
            {
                EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
                playerStats.DoMagicalDamage(enemyTarget);
            }
        }
    }
}
