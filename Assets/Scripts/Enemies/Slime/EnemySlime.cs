using System;
using Core;
using Core.Interfaces;
using Core.ObjectPool;
using Core.ObjectPool.Configs.Enemies.Core.ObjectPool.Configs;
using Enemies.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Slime
{
    public class EnemySlime : Enemy, IAttackable, IStunnable, IMinionSpawner
    {
        private const float Knockback_Cancel_Delay = 1.5f;

        [Header("Slime Specific")] 
        [SerializeField] private SlimePoolConfig config;

        #region States
        public EnemyState AttackState { get; private set; }
        public EnemyState StunnedState { get; private set; }
    
        #endregion
        
        public event Action<Enemy> OnMinionSpawned;
        
        protected override void Awake()
        {
            base.Awake();

            IdleState = new SlimeIdleState(this, StateMachine, AnimatorHashes.EnemyIdleState);
            MoveState = new SlimeMoveState(this, StateMachine, AnimatorHashes.EnemyMoveState);
            BattleState = new SlimeBattleState(this, StateMachine, AnimatorHashes.EnemyBattleState);
            AttackState = new SlimeAttackState(this, StateMachine, AnimatorHashes.EnemyAttackState);
            StunnedState = new SlimeStunnedState(this, StateMachine, AnimatorHashes.EnemyStunnedState);
            DeadState = new SlimeDeadState(this, StateMachine, AnimatorHashes.EnemyIdleState);
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

            if (config.SlimeType == SlimeType.Small)
            {
                PoolManager.Instance.ReturnWithDelay(gameObject, config.ReturnToPoolDelay); 
                return;
            }

            CreateSlimes(config.SlimesToCreate, config.ChildSlimeConfig.Prefab);
            PoolManager.Instance.ReturnWithDelay(gameObject, config.ReturnToPoolDelay);
        }

        public void SetupSlime(int _facingDir)
        {
            if (_facingDir != FacingDir)
                Flip();

            float xVelocity = Random.Range(config.MinCreationVelocity.x, config.MaxCreationVelocity.x);
            float yVelocity = Random.Range(config.MinCreationVelocity.y, config.MaxCreationVelocity.y);

            isKnocked = true;

            GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -FacingDir, yVelocity);
            Invoke(nameof(CancelKnockBack), Knockback_Cancel_Delay);
        }
        
        private void CreateSlimes(int _amountOfSlimes, GameObject _slimePrefab)
        {
            for (int i = 0; i < _amountOfSlimes; i++)
            {
                GameObject newSlime = PoolManager.Instance.Spawn(_slimePrefab, transform.position, Quaternion.identity);
                
                EnemySlime slimeScript = newSlime.GetComponent<EnemySlime>();
                slimeScript.SetupSlime(FacingDir);
                
                OnMinionSpawned?.Invoke(slimeScript);
            }
        }
    }
}
