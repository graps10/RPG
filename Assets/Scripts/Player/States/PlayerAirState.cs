namespace Player.States
{
    public class PlayerAirState : PlayerState
    {
        private const float Air_Control_Multiplier = 0.8f;
        
        public PlayerAirState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Update()
        {
            base.Update();

            if(player.IsWallDetected())
                stateMachine.ChangeState(player.WallSlide);

            if(player.IsGroundDetected())
                stateMachine.ChangeState(player.IdleState);

            if(xInput != 0)
                player.SetVelocity(player.GetMoveSpeed() * Air_Control_Multiplier * xInput, rb.velocity.y);
        }
    }
}
