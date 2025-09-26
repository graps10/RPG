using UnityEngine;

namespace Player.States
{
    public class PlayerAimSwordState : PlayerState
    {
        private const float Exit_Busy_Duration = 0.2f;
        
        public PlayerAimSwordState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            player.Skill.Sword.DotsActive(true);
            if(player.IsBusy())
                stateMachine.ChangeState(player.IdleState);
        }
        
        public override void Update()
        {
            base.Update();

            player.SetZeroVelocity();

            if(Input.GetKeyUp(KeyCode.Mouse1))
                stateMachine.ChangeState(player.IdleState);

            if (Camera.main == null) return;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(player.transform.position.x > mousePosition.x && player.FacingDir == 1 
               || player.transform.position.x < mousePosition.x && player.FacingDir == -1)
                player.Flip();
        }
        
        public override void Exit()
        {
            base.Exit();

            player.StartCoroutine(nameof(Player.BusyFor), Exit_Busy_Duration);
        }
    }
}
