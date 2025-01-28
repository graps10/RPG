using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision) 
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(collision.GetComponent<Enemy>() != null)
        {
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            playerStats.DoMagicalDamage(enemyTarget);
        }
    }
}
