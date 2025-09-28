using Components.Audio;
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

            AudioManager.Instance.PlaySFX(SFXEnum.PlayerFootsteps);
        }

        public override void Exit()
        {
            base.Exit();

            AudioManager.Instance.StopSFX(SFXEnum.PlayerFootsteps);
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
