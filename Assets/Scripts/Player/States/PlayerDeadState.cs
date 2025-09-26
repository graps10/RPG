using UI_Elements;
using UnityEngine;

namespace Player.States
{
    public class PlayerDeadState : PlayerState
    {
        public PlayerDeadState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            GameObject.Find("Canvas").GetComponent<UI>().SwitchOnEndScreen();
        }

        public override void Update()
        {
            base.Update();

            player.SetZeroVelocity();
        }
    }
}
