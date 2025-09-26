using Controllers.Skill_Controllers;
using UnityEngine;

namespace Player.States
{
    public class PlayerGroundedState : PlayerState
    {
        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Update()
        {
            base.Update();
            
            HandleBlackHoleInput();
            HandleSwordInput();
            HandleParryInput();
            HandlePrimaryAttackInput();
            HandleJumpInput();
            HandleAirTransition();
        }
        
        private void HandleBlackHoleInput()
        {
            if (Input.GetKeyDown(KeyCode.R) && IsBlackHoleUnlocked())
            {
                if (player.Skill.BlackHole.GetCooldownRemaining() > 0)
                {
                    player.Fx.CreatePopUpText("Cooldown");
                    return;
                }

                stateMachine.ChangeState(player.BlackHoleState);
            }
        }
        private void HandleSwordInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && IsSwordUnlocked())
            {
                if (IsSwordWasThrown())
                    player.Sword.GetComponent<SwordSkillController>()?.ReturnSword();
                else
                    stateMachine.ChangeState(player.AimSword);
            }
        }

        private void HandleParryInput()
        {
            if (Input.GetKeyDown(KeyCode.Q) && !IsSwordWasThrown() && IsParryUnlocked())
            {
                if (!player.Skill.Parry.CanUseSkill())
                {
                    Debug.Log("Can't use parry");
                    return;
                }

                Debug.Log("Used parry");
                stateMachine.ChangeState(player.CounterAttack);
            }
        }

        private void HandlePrimaryAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !IsSwordWasThrown())
                stateMachine.ChangeState(player.PrimaryAttack);
        }

        private void HandleJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
                stateMachine.ChangeState(player.JumpState);
        }

        private void HandleAirTransition()
        {
            if (!player.IsGroundDetected())
                stateMachine.ChangeState(player.AirState);
        }

        private bool IsSwordWasThrown()
        {
            return player.Sword != null;
        }

        private bool IsSwordUnlocked() => player.Skill.Sword.IsSwordUnlocked();
        private bool IsParryUnlocked() => player.Skill.Parry.IsParryUnlocked();
        private bool IsBlackHoleUnlocked() => player.Skill.BlackHole.IsBlackHoleUnlocked();
    }
}
