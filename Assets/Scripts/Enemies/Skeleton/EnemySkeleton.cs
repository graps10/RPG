using Core;
using Core.Interfaces;
using Enemies.Base;
using UnityEngine;

namespace Enemies.Skeleton
{
    public class EnemySkeleton : Enemy, IAttackable, IStunnable
    {
        #region States
        
        public EnemyState AttackState { get; private set; }
        public EnemyState StunnedState { get; private set; }
    
        #endregion

        protected override void Awake()
        {
            base.Awake();

            IdleState = new SkeletonIdleState(this, StateMachine, AnimatorHashes.EnemyIdleState);
            MoveState = new SkeletonMoveState(this, StateMachine, AnimatorHashes.EnemyMoveState);
            BattleState = new SkeletonBattleState(this, StateMachine, AnimatorHashes.EnemyBattleState);
            AttackState = new SkeletonAttackState(this, StateMachine, AnimatorHashes.EnemyAttackState);
            StunnedState = new SkeletonStunnedState(this, StateMachine, AnimatorHashes.EnemyStunnedState);
            DeadState = new SkeletonDeadState(this, StateMachine, AnimatorHashes.EnemyIdleState);
        }

        protected override void Start()
        {
            base.Start();

            StateMachine.Initialize(IdleState);
        }

        public bool CanBeStunned()
        {
            if (!canBeStunned) return 
                false;
            
            CloseCounterAttackWindow();
            StateMachine.ChangeState(StunnedState);
            return true;
        }
        
        public bool CanAttack()
        {
            if (Time.time >= GetLastTimeAttacked() + GetAttackCooldown())
            { 
                SetAttackCooldown(Random.Range(GetAttackCooldownRange().x, GetAttackCooldownRange().y));
                SetLastTimeAttacked(Time.time);
                return true;
            }

            return false;
        }

        public override void Die()
        {
            base.Die();

            StateMachine.ChangeState(DeadState);
        }
    }
}
