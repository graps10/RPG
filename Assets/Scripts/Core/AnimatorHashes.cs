using UnityEngine;

namespace Core
{
    public static class AnimatorHashes
    {
        #region Trigger

        public static int Hit = Animator.StringToHash("Hit");
        
        public static int Explode = Animator.StringToHash("Explode");
        
        public static int StunFold = Animator.StringToHash("StunFold");

        public static int FadeOut = Animator.StringToHash("fadeOut");
        
        public static int FadeIn = Animator.StringToHash("fadeIn");

        #endregion

        #region Int

        public static int AttackNumber = Animator.StringToHash("AttackNumber");
        
        public static int ComboCounter = Animator.StringToHash("ComboCounter");

        #endregion

        #region Float

        public static int XVelocity = Animator.StringToHash("xVelocity");
        public static int YVelocity = Animator.StringToHash("yVelocity");

        #endregion

        #region Bool

        public static int Rotation = Animator.StringToHash("Rotation");
        
        public static int ActiveCheckpoint = Animator.StringToHash("active");
        
        public static int SuccessfulCounterAttack = Animator.StringToHash("SuccessfulCounterAttack");

        #endregion

        #region Enemy State Machine

        public static int EnemyIdleState = Animator.StringToHash("Idle");
        public static int EnemyMoveState = Animator.StringToHash("Move");
        public static int EnemyBattleState = Animator.StringToHash("Battle");
        public static int EnemyRushState = Animator.StringToHash("MoveFast");
        public static int EnemyAttackState = Animator.StringToHash("Attack");
        public static int EnemyStunnedState = Animator.StringToHash("Stunned");
        public static int EnemyJumpState = Animator.StringToHash("Jump");
        public static int EnemyTeleportState = Animator.StringToHash("Teleport");
        public static int EnemySpellCastState = Animator.StringToHash("SpellCast");
        public static int EnemyDeadState = Animator.StringToHash("Dead");

        #endregion
        
        #region Player State Machine

        public static int PlayerIdleState = Animator.StringToHash("Idle");
        public static int PlayerMoveState = Animator.StringToHash("Move");
        
        public static int PlayerJumpState = Animator.StringToHash("Jump");
        public static int PlayerAirState = Animator.StringToHash("Jump");
        public static int PlayerDashState = Animator.StringToHash("Dash");
        
        public static int PlayerWallSlideState = Animator.StringToHash("WallSlide");
        public static int PlayerWallJumpState = Animator.StringToHash("Jump");
        
        public static int PlayerPrimaryAttackState = Animator.StringToHash("Attack");
        public static int PlayerCounterAttackState = Animator.StringToHash("CounterAttack");
        
        public static int PlayerAimSwordState = Animator.StringToHash("AimSword");
        public static int PlayerCatchSwordState = Animator.StringToHash("CatchSword");
        public static int PlayerBlackHoleState = Animator.StringToHash("Jump");
        
        public static int PlayerDeadState = Animator.StringToHash("Die");

        #endregion
    }
}