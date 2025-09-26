using Core;
using Core.Interfaces;
using Enemies.Base;
using UnityEngine;

namespace Enemies.Slime
{
    public class EnemySlime : Enemy, IAttackable, IStunnable
    {
        private const float Knockback_Cancel_Delay = 1.5f;
        
        [Header("Slime Specific")]
        [SerializeField] private SlimeType slimeType;
        [SerializeField] private int slimesToCreate;
        [SerializeField] private GameObject slimePrefab;
        [SerializeField] private Vector2 minCreationVelocity;
        [SerializeField] private Vector2 maxCreationVelocity;

        #region States
        public EnemyState AttackState { get; private set; }
        public EnemyState StunnedState { get; private set; }
    
        #endregion

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

            if (slimeType == SlimeType.Small)
                return;

            CreateSlimes(slimesToCreate, slimePrefab);
        }

        public void SetupSlime(int _facingDir)
        {
            if (_facingDir != FacingDir)
                Flip();

            float xVelocity = Random.Range(minCreationVelocity.x, maxCreationVelocity.x);
            float yVelocity = Random.Range(minCreationVelocity.y, maxCreationVelocity.y);

            isKnocked = true;

            GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -FacingDir, yVelocity);
            Invoke(nameof(CancelKnockBack), Knockback_Cancel_Delay);
        }

        private void CreateSlimes(int _amountOfSlimes, GameObject _slimePrefab)
        {
            for (int i = 0; i < _amountOfSlimes; i++)
            {
                GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);
                newSlime.GetComponent<EnemySlime>().SetupSlime(FacingDir);
            }
        }
    }
}
