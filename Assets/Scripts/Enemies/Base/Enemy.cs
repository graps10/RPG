using System.Collections;
using Components.FX;
using Items_and_Inventory;
using Stats;
using UnityEngine;

namespace Enemies.Base
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(EnemyStats))]
    [RequireComponent(typeof(EntityFX))]
    [RequireComponent(typeof(ItemDrop))]
    public class Enemy : Entity.Entity
    {
        [SerializeField] protected LayerMask whatIsPlayer;

        [SerializeField] private float playerDetectionRange = 50f;
        [SerializeField] private float wallDetectionRange = 50f;
        
        [Header("Stunned Info")]
        [SerializeField] protected float stunDuration = 1;
        [SerializeField] protected Vector2 stunDirection = new(10, 12);
        [SerializeField] protected GameObject counterImage;

        [Header("Move Info")]
        [SerializeField] protected float moveSpeed = 1.5f;
        [SerializeField] protected float idleTime = 2;
        [SerializeField] protected float battleTime = 7;
    
        [Header("Attack Info")]
        [SerializeField] protected float agroDistance = 2;
        [SerializeField] protected float attackDistance = 2;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected Vector2 attackCooldownRange = new(1f, 2f);
        
        #region Common State Properties
        
        public EnemyState IdleState { get; protected set; }
        public EnemyState MoveState { get; protected set; }
        public EnemyState BattleState { get; protected set; }
        public EnemyState DeadState { get; protected set; }
        
        #endregion
        
        public int LastAnimBoolName { get; private set; }
        
        public EntityFX Fx { get; private set; }
        
        protected EnemyStateMachine StateMachine { get; private set; }
        
        protected bool canBeStunned;
        protected float lastTimeAttacked;
        
        private float _defaultMoveSpeed;

        protected override void Awake()
        {
            base.Awake();

            StateMachine = new EnemyStateMachine();
            _defaultMoveSpeed = moveSpeed;
        }

        protected override void Start()
        {
            base.Start();

            Fx = GetComponent<EntityFX>();
        }
        
        protected override void Update()
        {
            base.Update();

            StateMachine.CurrentState.Update();
        }

        public override void SlowEntityBy(float slowPercantage, float slowDuration)
        {
            moveSpeed *= (1 - slowPercantage);
            Anim.speed *= (1 - slowPercantage);

            Invoke(nameof(ReturnDefaultSpeed), slowDuration);
        }

        protected override void ReturnDefaultSpeed()
        {
            base.ReturnDefaultSpeed();

            moveSpeed = _defaultMoveSpeed;
        }

        public void AssignLastAnimName(int animBoolName) 
            => LastAnimBoolName = animBoolName;

        public void FreezeTime(bool timeFrozen)
        {
            if (timeFrozen)
            {
                moveSpeed = 0;
                Anim.speed = 0;
            }
            else
            {
                moveSpeed = _defaultMoveSpeed;
                Anim.speed = 1;
            }
        }

        public virtual void FreezeTimeFor(float _duration) 
            => StartCoroutine(FreezeTimeCoroutine(_duration));

        protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
        {
            FreezeTime(true);

            yield return new WaitForSeconds(_seconds);

            FreezeTime(false);
        }

        #region Counter attack window
        
        public virtual void OpenCounterAttackWindow()
        {
            canBeStunned = true;
            counterImage.SetActive(true);
        }
        public virtual void CloseCounterAttackWindow()
        {
            canBeStunned = false;
            counterImage.SetActive(false);
        }
        
        #endregion
        
        public virtual void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

        public virtual void AnimationSpecialAttackTrigger() { }

        public virtual RaycastHit2D IsPlayerDetected()
        {
            RaycastHit2D playerDetected = Physics2D.Raycast(wallCheck.position, 
                Vector2.right * FacingDir, playerDetectionRange, whatIsPlayer);
            RaycastHit2D wallDetected = Physics2D.Raycast(wallCheck.position, 
                Vector2.right * FacingDir, wallDetectionRange, whatIsGround);

            if (wallDetected)
            {
                if (wallDetected.distance < playerDetected.distance)
                    return default;
            }

            return playerDetected;
        }

        #region Public Getters

        public float GetStunDuration() => stunDuration;
        public Vector2 GetStunDirection() => stunDirection;
        
        public float GetMoveSpeed() => moveSpeed;
        public float GetIdleTime() => idleTime;
        public float GetBattleTime() => battleTime;
        
        public float GetAgroDistance() => agroDistance;
        public float GetAttackDistance() => attackDistance;
        public float GetAttackCooldown() => attackCooldown;

        public Vector2 GetAttackCooldownRange() => attackCooldownRange;
        
        public float GetLastTimeAttacked() => lastTimeAttacked;

        #endregion
        
        public void SetMoveSpeed(float value) => moveSpeed = value;
        public void SetAttackCooldown(float value) => attackCooldown = value;
        public void SetLastTimeAttacked(float value) => lastTimeAttacked = value;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, 
                new Vector3(transform.position.x + attackDistance * FacingDir, transform.position.y));
        }
    }
}