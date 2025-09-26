using Core;
using UnityEngine;

namespace Enemies.Base.States
{
    public class EnemyStunnedState<TEnemy> : EnemyState<TEnemy> where TEnemy : Enemy
    {
        public EnemyStunnedState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Enter()
        {
            base.Enter();

            enemy.Fx?.InvokeBlinkEffect(COLOR_BLINK_REPEAT_RATE);
            stateTimer = enemy.GetStunDuration();
            rb.velocity = new Vector2(-enemy.FacingDir * enemy.GetStunDirection().x, enemy.GetStunDirection().y);
        }
        
        public override void Update()
        {
            base.Update();

            if (stateTimer < 0)
                stateMachine.ChangeState(enemy.IdleState);
        }
        
        public override void Exit()
        {
            base.Exit();

            enemy.Fx?.InvokeCancelColorChange(0);
        }
    }
}