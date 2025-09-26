using Core;
using UnityEngine;

namespace Player.States
{
    public class PlayerPrimaryAttackState : PlayerState
    {
        private const float Combo_Window = 2f; // for combo reset timing
        private const float Exit_Busy_Duration = 0.15f; // for BusyFor coroutine
        private const float State_Timer_Duration = 0.1f; // for state timer
        private const int Max_Combo_Count = 2; // maximum combo index before reset
        
        public int ComboCounter { get; private set; }
        
        private float _lastTimeAttacked;

        public PlayerPrimaryAttackState(Player player, PlayerStateMachine stateMachine, int animBoolName) :
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            // AudioManager.instance.PlaySFX(1);

            xInput = 0; // for attack direction bug

            if (ComboCounter > Max_Combo_Count || Time.time >= _lastTimeAttacked + Combo_Window)
                ComboCounter = 0;

            player.Anim.SetInteger(AnimatorHashes.ComboCounter, ComboCounter);

            float attackDir = player.FacingDir;
            if (xInput != 0)
                attackDir = xInput;

            player.SetVelocity(player.GetAttackMovement()[ComboCounter].x * attackDir,
                player.GetAttackMovement()[ComboCounter].y);
        }
        
        public override void Exit()
        {
            base.Exit();

            player.StartCoroutine(nameof(Player.BusyFor), Exit_Busy_Duration);

            ComboCounter++;
            _lastTimeAttacked = Time.time;

            stateTimer = State_Timer_Duration;
        }
        
        public override void Update()
        {
            base.Update();

            if (stateTimer < 0)
                player.SetZeroVelocity();

            if (triggerCalled)
                player.StateMachine.ChangeState(player.IdleState);
        }
    }
}
