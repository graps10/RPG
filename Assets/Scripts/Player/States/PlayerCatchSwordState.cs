using UnityEngine;

namespace Player.States
{
    public class PlayerCatchSwordState : PlayerState
    {
        private const float Exit_Busy_Duration = 0.2f;

        public PlayerCatchSwordState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            var sword = player.Sword.transform;

            player.Fx.PlayDustFX();
            player.Fx.ScreenShake(player.Fx.GetShakeSwordImpact());

            if (player.transform.position.x > sword.position.x && player.FacingDir == 1 || 
                player.transform.position.x < sword.position.x && player.FacingDir == -1)
                player.Flip();

            rb.velocity = new Vector2(player.GetSwordReturnImpact() * -player.FacingDir, rb.velocity.y);
        }
        
        public override void Update()
        {
            base.Update();

            if (triggerCalled)
                stateMachine.ChangeState(player.IdleState);
        }
        
        public override void Exit()
        {
            base.Exit();

            player.StartCoroutine(nameof(Player.BusyFor), Exit_Busy_Duration);
        }
    }
}
