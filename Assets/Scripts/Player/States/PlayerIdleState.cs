using UnityEngine;

namespace Player.States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
        
            player.SetZeroVelocity();
        }

        public override void Update()
        {
            base.Update();
            
            if(Mathf.Approximately(xInput, player.FacingDir) && player.IsWallDetected())
                return;
        
            if(xInput != 0 && !player.IsBusy())
                stateMachine.ChangeState(player.MoveState);
        }
    }
}