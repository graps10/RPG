using UnityEngine;

namespace Player.States
{
    public class PlayerWallSlideState : PlayerState
    {
        private const float Vertical_Slide_Multiplier = 0.7f;
        
        public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Update()
        {
            base.Update();

            if (!player.IsWallDetected())
                stateMachine.ChangeState(player.AirState);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                stateMachine.ChangeState(player.WallJump);
                return;
            }

            if (xInput != 0 && !Mathf.Approximately(player.FacingDir, xInput))
                stateMachine.ChangeState(player.IdleState);

            rb.velocity = yInput < 0 ? new Vector2(0, rb.velocity.y) : new Vector2(0, rb.velocity.y * Vertical_Slide_Multiplier);

            if (player.IsGroundDetected())
                stateMachine.ChangeState(player.IdleState);
        }
    }
}
