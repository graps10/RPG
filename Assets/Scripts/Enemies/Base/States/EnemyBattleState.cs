using Core;
using Core.Interfaces;
using Managers;
using UnityEngine;

namespace Enemies.Base.States
{
    public class EnemyBattleState<TEnemy> : EnemyState<TEnemy> where TEnemy : Enemy
    {
        protected const float ATTACK_DISTANCE_THRESHOLD = 0.5f;
        protected const float MAX_CHASE_DISTANCE = 10f;
        
        protected int moveDir;
        protected bool flippedOnce;
        
        public EnemyBattleState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Enter()
        {
            base.Enter();

            if (PlayerManager.Instance.PlayerStats.IsDead)
                stateMachine.ChangeState(enemy.MoveState);
            
            stateTimer = enemy.GetBattleTime();
            flippedOnce = false;
        }

        protected virtual void HandleBattleBehavior()
        {
            if (enemy.IsPlayerDetected())
            {
                stateTimer = enemy.GetBattleTime();

                if (enemy.IsPlayerDetected().distance < enemy.GetAttackDistance())
                {
                    if (enemy is IAttackable attackable && attackable.CanAttack())
                        stateMachine.ChangeState(attackable.AttackState);
                }
            }
            else
            {
                if (!flippedOnce)
                {
                    flippedOnce = true;
                    enemy.Flip();
                }

                if (CanReturnToIdle())
                    stateMachine.ChangeState(enemy.IdleState);
            }
        }

        protected virtual void ChasePlayer()
        {
            if (!enemy.IsPlayerDetected()) 
                return;
            
            if (enemy.IsPlayerDetected().distance < enemy.GetAttackDistance() - ATTACK_DISTANCE_THRESHOLD)
                enemy.Anim.SetFloat(AnimatorHashes.XVelocity, 0);
            else
            {
                enemy.Anim.SetFloat(AnimatorHashes.XVelocity, moveDir);
                enemy.SetVelocity(enemy.GetMoveSpeed() * moveDir, rb.velocity.y);
            }
        }
        
        protected bool CanReturnToIdle()
        {
            bool battleTimeExpired = stateTimer < 0;
            bool playerTooFar = 
                Vector2.Distance(PlayerManager.Instance.PlayerGameObject.transform.position, enemy.transform.position) > MAX_CHASE_DISTANCE;

            return battleTimeExpired || playerTooFar;
        }

        protected virtual void CalculateMoveDirection()
        {
            moveDir = PlayerManager.Instance.PlayerGameObject.transform.position.x 
                      > enemy.transform.position.x ? 1 : -1;
        }
    }
}