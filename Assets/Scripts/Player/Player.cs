using System.Collections;
using Components.FX;
using Core;
using Items_and_Inventory;
using Player.States;
using Skills;
using UnityEngine;

namespace Player
{
    public class Player : Entity.Entity
    {
        [Header("Attack Details")]
        [SerializeField] private Vector2[] attackMovement;
        [SerializeField] private float counterAttackDuration = 0.2f;
        
        [Header("Move Info")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float jumpForce;
        [SerializeField] private float swordReturnImpact;
        
        [Header("Dash Info")]
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashDuration;
        
        #region States
    
        public PlayerStateMachine StateMachine { get; private set; }

        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerAirState AirState { get; private set; }
        public PlayerDashState DashState { get; private set; }
    
        public PlayerWallSlideState WallSlide { get; private set; }
        public PlayerWallJumpState WallJump { get; private set; }

        public PlayerPrimaryAttackState PrimaryAttack { get; private set; }
        public PlayerCounterAttackState CounterAttack { get; private set; }

        public PlayerAimSwordState AimSword { get; private set; }
        public PlayerCatchSwordState CatchSword { get; private set; }
        public PlayerBlackHoleState BlackHoleState { get; private set; }

        public PlayerDeadState DeadState { get; private set; }
    
        #endregion
        
        public SkillManager Skill { get; private set; }
        public GameObject Sword { get; private set; }

        public PlayerFX Fx { get; private set; }
        
        private bool _isBusy;
        
        private float _defaultDashSpeed;
        private float _defaultMoveSpeed;
        private float _defaultJumpForce;
        
        private float _dashDir;

        protected override void Awake()
        {
            base.Awake();

            StateMachine = new PlayerStateMachine();

            IdleState = new PlayerIdleState(this, StateMachine, AnimatorHashes.PlayerIdleState);
            MoveState = new PlayerMoveState(this, StateMachine, AnimatorHashes.PlayerMoveState);
            JumpState = new PlayerJumpState(this, StateMachine, AnimatorHashes.PlayerJumpState);
            AirState = new PlayerAirState(this, StateMachine, AnimatorHashes.PlayerAirState);
            DashState = new PlayerDashState(this, StateMachine, AnimatorHashes.PlayerDashState);
            
            WallSlide = new PlayerWallSlideState(this, StateMachine, AnimatorHashes.PlayerWallSlideState);
            WallJump = new PlayerWallJumpState(this, StateMachine, AnimatorHashes.PlayerWallJumpState);

            PrimaryAttack = new PlayerPrimaryAttackState(this, StateMachine, AnimatorHashes.PlayerPrimaryAttackState);
            CounterAttack = new PlayerCounterAttackState(this, StateMachine, AnimatorHashes.PlayerCounterAttackState);

            AimSword = new PlayerAimSwordState(this, StateMachine, AnimatorHashes.PlayerAimSwordState);
            CatchSword = new PlayerCatchSwordState(this, StateMachine, AnimatorHashes.PlayerCatchSwordState);
            BlackHoleState = new PlayerBlackHoleState(this, StateMachine, AnimatorHashes.PlayerBlackHoleState);

            DeadState = new PlayerDeadState(this, StateMachine, AnimatorHashes.PlayerDeadState);
        }

        protected override void Start()
        {
            base.Start();

            Fx = GetComponent<PlayerFX>();

            Skill = SkillManager.Instance;

            StateMachine.Initialize(IdleState);

            _defaultMoveSpeed = moveSpeed;
            _defaultJumpForce = jumpForce;
            _defaultDashSpeed = dashSpeed;
        }

        protected override void Update()
        {
            if (Time.timeScale == 0) 
                return;

            base.Update();

            StateMachine.CurrentState.Update();

            CheckForDashInput();

            if (Input.GetKeyDown(KeyCode.F) && Skill.Crystal.IsCrystalUnlocked())
                Skill.Crystal.CanUseSkill();

            if (Input.GetKeyDown(KeyCode.E))
                Inventory.Instance.UseFlask();
        }

        public override void Die()
        {
            base.Die();

            StateMachine.ChangeState(DeadState);
        }

        public override void SlowEntityBy(float slowPercantage, float slowDuration)
        {
            moveSpeed *= (1 - slowPercantage);
            jumpForce *= (1 - slowPercantage);
            dashSpeed *= (1 - slowPercantage);
            Anim.speed *= (1 - slowPercantage);

            Invoke(nameof(ReturnDefaultSpeed), slowDuration);
        }

        protected override void ReturnDefaultSpeed()
        {
            base.ReturnDefaultSpeed();

            moveSpeed = _defaultMoveSpeed;
            jumpForce = _defaultJumpForce;
            dashSpeed = _defaultDashSpeed;
        }

        public override void SetupKnockbackDir(Transform _damageDirection)
        {
            knockbackPower = Vector2.zero;
        }

        public void AssignNewSword(GameObject newSword)
        {
            Sword = newSword;
        }
        
        public void CatchTheSword()
        {
            StateMachine.ChangeState(CatchSword);
            Destroy(Sword.gameObject);
            Sword = null;
        }

        public IEnumerator BusyFor(float seconds)
        {
            _isBusy = true;
            yield return new WaitForSeconds(seconds);
            _isBusy = false;
        }

        public void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

        private void CheckForDashInput()
        {
            if (IsWallDetected())
                return;

            if (Skill.Dash.IsDashUnlocked() == false)
                return;

            if (StateMachine.CurrentState == BlackHoleState || StateMachine.CurrentState == DeadState)
                return;
            
            if (!Input.GetKeyDown(KeyCode.LeftShift) || !SkillManager.Instance.Dash.CanUseSkill()) 
                return;
            
            _dashDir = Input.GetAxisRaw("Horizontal");

            if (_dashDir == 0)
                _dashDir = FacingDir;

            StateMachine.ChangeState(DashState);
        }

        #region Public Getters

        public bool IsBusy() => _isBusy;
        public Vector2[] GetAttackMovement() => attackMovement;
        public float GetCounterAttackDuration() => counterAttackDuration;
        
        public float GetMoveSpeed() => moveSpeed;
        public float GetJumpForce() => jumpForce;
        
        public float GetSwordReturnImpact() => swordReturnImpact;
        
        public float GetDashSpeed() => dashSpeed;
        public float GetDashDuration() => dashDuration;
        public float GetDishDirection() => _dashDir;

        #endregion
    }
}
