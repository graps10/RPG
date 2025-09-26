using Managers;

namespace Player.States
{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            AudioManager.Instance.PlaySFX(13, null);
        }

        public override void Exit()
        {
            base.Exit();

            AudioManager.Instance.StopSFX(13);
        }

        public override void Update()
        {
            base.Update();

            player.SetVelocity(xInput * player.GetMoveSpeed(), rb.velocity.y);

            if (xInput == 0 || player.IsWallDetected())
                stateMachine.ChangeState(player.IdleState);
        }
    }
}
