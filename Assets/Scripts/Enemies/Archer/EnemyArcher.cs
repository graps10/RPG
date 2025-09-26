using Controllers;
using Core;
using Core.Interfaces;
using Core.Utilities;
using Enemies.Base;
using Managers;
using UnityEngine;

namespace Enemies.Archer
{
    public class EnemyArcher : Enemy, IAttackable, IStunnable, IJumpable
    {
        [Header("Archer Specific")]
        [SerializeField] private float arrowSpeed;
        [SerializeField] private Vector2 jumpVelocity;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float safeDistance; // how close player should be to trigger jump on battle state
        [SerializeField] private float lastTimeJumped;

        [Header("Additional Collision Check")]
        [SerializeField] private Transform groundBehindCheck;
        [SerializeField] private Vector2 groundBehindCheckSize;
        [SerializeField] private float wallBehindExtraDistance = 2f;
        
        #region States
        
        public EnemyState AttackState { get; private set; }
        public EnemyState StunnedState { get; private set; }
        public EnemyState JumpState { get; private set; }
    
        #endregion

        protected override void Awake()
        {
            base.Awake();

            IdleState = new ArcherIdleState(this, StateMachine, AnimatorHashes.EnemyIdleState);
            MoveState = new ArcherMoveState(this, StateMachine, AnimatorHashes.EnemyMoveState);
            BattleState = new ArcherBattleState(this, StateMachine, AnimatorHashes.EnemyBattleState);
            AttackState = new ArcherAttackState(this, StateMachine, AnimatorHashes.EnemyAttackState);
            StunnedState = new ArcherStunnedState(this, StateMachine, AnimatorHashes.EnemyStunnedState);
            JumpState = new ArcherJumpState(this, StateMachine, AnimatorHashes.EnemyJumpState);
            DeadState = new ArcherDeadState(this, StateMachine, AnimatorHashes.EnemyIdleState);
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
        
        public bool CanJump()
        {
            if (GroundBehind() == false || WallBehind())
                return false;

            if (Time.time >= lastTimeJumped + jumpCooldown)
            {
                lastTimeJumped = Time.time;
                return true;
            }

            return false;
        }

        public override void Die()
        {
            base.Die();

            StateMachine.ChangeState(DeadState);
        }

        public override void AnimationSpecialAttackTrigger()
        {
            var rotation = FacingDir == 1 ? Vector3.zero : TransformUtils.FlipAngle;
            Quaternion arrowRotation = Quaternion.Euler(rotation);

            GameObject newArrow = PoolManager.Instance.Spawn("arrow", attackCheck.position, arrowRotation);
            newArrow.GetComponent<ArrowController>().SetupArrow(Stats, arrowSpeed * FacingDir);
        }

        public Vector2 GetJumpVelocity() => jumpVelocity;
        public float GetSafeDistance() => safeDistance;
        
        public bool GroundBehind() 
            => Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 
                0, Vector2.zero, 0, whatIsGround);

        public bool WallBehind() 
            => Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDir, 
                wallCheckDistance + wallBehindExtraDistance, whatIsGround);
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
        }
    }
}
