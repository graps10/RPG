using Controllers;
using Core;
using Core.Interfaces;
using Core.ObjectPool.Configs.Enemies;
using Enemies.Base;
using UnityEngine;

namespace Enemies.Shady
{
    public class EnemyShady : Enemy, IStunnable
    {
        [Header("Shady Specific")]
        [SerializeField] private ShadyPoolConfig config;

        #region States
        public EnemyState StunnedState { get; private set; }
    
        #endregion

        protected override void Awake()
        {
            base.Awake();

            IdleState = new ShadyIdleState(this, StateMachine, AnimatorHashes.EnemyIdleState);
            MoveState = new ShadyMoveState(this, StateMachine, AnimatorHashes.EnemyMoveState);
            BattleState = new ShadyBattleState(this, StateMachine, AnimatorHashes.EnemyRushState);
            StunnedState = new ShadyStunnedState(this, StateMachine, AnimatorHashes.EnemyStunnedState);
            DeadState = new ShadyDeadState(this, StateMachine, AnimatorHashes.EnemyDeadState);
        }

        public bool CanBeStunned()
        {
            if (!canBeStunned) return 
                false;
            
            CloseCounterAttackWindow();
            StateMachine.ChangeState(StunnedState);
            return true;
        }

        public override void Die()
        {
            base.Die();
            StateMachine.ChangeState(DeadState);
        }

        public override void AnimationSpecialAttackTrigger()
        {
            GameObject newExplosive = Instantiate(config.ExplosivePrefab, attackCheck.position, Quaternion.identity);

            newExplosive.GetComponent<ExplosiveController>()
                .SetupExplosive(Stats, config.GrowSpeed, config.MaxSize, attackCheckRadius);

            Cd.enabled = false;
            Rb.gravityScale = 0;
        }

        public float GetBattleStateMoveSpeed() => config.BattleStateMoveSpeed;
        public void SelfDestroy() => Destroy(gameObject);
    }
}
