using UnityEngine;

namespace Player.States
{
    public class PlayerJumpState : PlayerState
    {
        public PlayerJumpState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }
        
        public override void Enter()
        {
            base.Enter();
            rb.velocity = new Vector2(rb.velocity.x, player.GetJumpForce());
        }

        public override void Update()
        {
            base.Update();

            rb.velocity = new Vector2(player.GetMoveSpeed() * xInput, rb.velocity.y);

            if(rb.velocity.y < 0)
                stateMachine.ChangeState(player.AirState);
        }
    }
}
