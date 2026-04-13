using Core;
using Core.Interfaces;
using Managers;
using UnityEngine;

namespace Enemies.Base.States
{
    public class EnemyBattleState<TEnemy> : EnemyState<TEnemy> where TEnemy : Enemy
    {
        protected const float ATTACK_DISTANCE_THRESHOLD = 0.5f;
        protected const float MAX_CHASE_DISTANCE = 15f;
        protected const float FLIP_DISTANCE_THRESHOLD = 0.5f;
        
        protected int moveDir;
        
        public EnemyBattleState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Enter()
        {
            base.Enter();

            if (PlayerManager.Instance.PlayerGameObject.Stats.IsDead)
                stateMachine.ChangeState(enemy.MoveState);
            
            stateTimer = enemy.GetBattleTime();
            CalculateMoveDirection();
            if (moveDir != 0 && moveDir != enemy.FacingDir)
                enemy.Flip();
        }
        
        public override void Update()
        {
            base.Update();
            
            CalculateMoveDirection(); 

            HandleBattleBehavior();
            
            if (stateMachine.CurrentState == this)
                ChasePlayer();
        }

        protected virtual void HandleBattleBehavior()
        {
            if (moveDir != 0 && moveDir != enemy.FacingDir)
                enemy.Flip();
            
            var hit = enemy.IsPlayerDetected();

            if (hit)
            {
                stateTimer = enemy.GetBattleTime();
                
                if (hit.distance < enemy.GetAttackDistance())
                {
                    if (enemy is IAttackable attackable && attackable.CanAttack())
                        stateMachine.ChangeState(attackable.AttackState);
                }
            }
            else
            {
                if (CanReturnToIdle())
                    stateMachine.ChangeState(enemy.IdleState);
            }
        }

        protected virtual void ChasePlayer()
        {
            var hit = enemy.IsPlayerDetected();
            
            if (hit && hit.distance < enemy.GetAttackDistance() - ATTACK_DISTANCE_THRESHOLD)
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
                Vector2.Distance(PlayerManager.Instance.PlayerGameObject.transform.position, 
                    enemy.transform.position) > MAX_CHASE_DISTANCE;

            return battleTimeExpired || playerTooFar;
        }

        protected virtual void CalculateMoveDirection()
        {
            float xDistance = PlayerManager.Instance.PlayerGameObject.transform.position.x - enemy.transform.position.x;
            
            if (Mathf.Abs(xDistance) < FLIP_DISTANCE_THRESHOLD) 
                moveDir = enemy.FacingDir; 
            else
                moveDir = xDistance > 0 ? 1 : -1;
        }
    }
}