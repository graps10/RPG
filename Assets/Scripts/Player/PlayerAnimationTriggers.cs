using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    [SerializeField] private Player player;

    private void AnimatinTrigger()
    {
        player.AnimationTrigger();
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                
                player.stats.DoDamage(_target);

                WeaponEffect(_target.transform);
            }
        }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }

    private void WeaponEffect(Transform _target)
    {
        ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

        if(weaponData != null)
            weaponData.Effect(_target.transform);
    }
}
