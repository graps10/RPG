namespace Player.States
{
    public class PlayerDashState : PlayerState
    {
        public PlayerDashState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }
        public override void Enter()
        {
            base.Enter();

            player.Skill.Dash.CloneOnDash();
            stateTimer = player.GetDashDuration();
            player.Stats.MakeInvincible(true);
        }

        public override void Exit()
        {
            base.Exit();

            player.Skill.Dash.CloneOnArrival();
            player.SetVelocity(0, rb.velocity.y);
            player.Stats.MakeInvincible(false);
        }

        public override void Update()
        {
            base.Update();

            if (!player.IsGroundDetected() && player.IsWallDetected())
                stateMachine.ChangeState(player.WallSlide);

            player.SetVelocity(player.GetDashSpeed() * player.GetDishDirection(), 0);

            if (stateTimer < 0)
                stateMachine.ChangeState(player.IdleState);

            player.Fx.CreateAfterImage();
        }
    }
}
