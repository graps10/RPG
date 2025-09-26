namespace Player.States
{
    public class PlayerWallJumpState : PlayerState
    {
        private const float State_Timer_Duration = 0.2f;
        private const float Horizontal_Jump_Force = 5f;
        
        public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }
        
        public override void Enter()
        {
            base.Enter();
        
            stateTimer = State_Timer_Duration;
            player.SetVelocity(Horizontal_Jump_Force * -player.FacingDir, player.GetJumpForce());
        }

        public override void Update()
        {
            base.Update();

            if(stateTimer < 0)
                stateMachine.ChangeState(player.AirState);
            
            if(player.IsGroundDetected())
                stateMachine.ChangeState(player.IdleState);
        }
    }
}
