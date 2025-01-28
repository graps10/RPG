using UnityEngine;

[CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Data/Item Effect/Freeze enemies", order = 0)]
public class FreezeEnemies_Effect : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExucuteEffect(Transform _transform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerStats.CurrentHealth > playerStats.GetMaxHealthValue() * 0.1f) return;

        if(!Inventory.instance.CanUseArmor()) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);

        foreach(var hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
        }
    }
}
