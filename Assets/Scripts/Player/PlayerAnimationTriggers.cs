using Components.Audio;
using Enemies;
using Enemies.Base;
using Items_and_Inventory;
using Managers;
using Skills;
using Stats;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationTriggers : MonoBehaviour
    {
        [SerializeField] private Player player;

        private void AnimationTrigger()
        {
            player.AnimationTrigger();
        }
    
        private void AttackTrigger()
        {
            AudioManager.Instance.PlaySFX(SFXEnum.PlayerAttack1);

            Collider2D[] colliders = 
                Physics2D.OverlapCircleAll(player.GetAttackCheckTransform().position, player.GetAttackCheckRadius());

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                {
                    EnemyStats _target = hit.GetComponent<EnemyStats>();
                    player.Stats.DoDamage(_target);
                    WeaponEffect(_target.transform);
                }
            }
        }

        private void ThrowSword()
        {
            SkillManager.Instance.Sword.CreateSword();
        }

        private void WeaponEffect(Transform target)
        {
            ItemData_Equipment weaponData = Inventory.Instance.GetEquipment(EquipmentType.Weapon);

            if (weaponData != null)
                weaponData.Effect(target.transform);
        }
    }
}
