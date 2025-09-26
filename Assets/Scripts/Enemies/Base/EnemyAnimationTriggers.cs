using Stats;
using UnityEngine;

namespace Enemies.Base
{
    public class EnemyAnimationTriggers : MonoBehaviour
    {
        private Enemy _enemy => GetComponentInParent<Enemy>();

        private void AnimationTrigger()
        {
            _enemy.AnimationTrigger();
        }

        private void AttackTrigger()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_enemy.GetAttackCheckTransform().position, _enemy.GetAttackCheckRadius());

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Player.Player>() != null)
                {
                    PlayerStats target = hit.GetComponent<PlayerStats>();

                    _enemy.Stats.DoDamage(target);
                }
            }
        }

        private void SpecialAttackTrigger() => _enemy.AnimationSpecialAttackTrigger();
        private void OpenCounterWindow() => _enemy.OpenCounterAttackWindow();
        private void CloseCounterWindow() => _enemy.CloseCounterAttackWindow();
    }
}