using UnityEngine;

namespace Enemies.Base.States
{
    public class EnemyDeadState<TEnemy> : EnemyState<TEnemy> where TEnemy : Enemy
    {
        protected const float DEAD_STATE_TIMER = 0.15f;
        protected static readonly Vector2 DeadVelocity = new(0, 10);
        
        public EnemyDeadState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Enter()
        {
            base.Enter();

            enemy.Anim.SetBool(enemy.LastAnimBoolName, true);
            enemy.Anim.speed = 0;
            enemy.Cd.enabled = false;

            stateTimer = DEAD_STATE_TIMER;
        }

        public override void Update()
        {
            base.Update();
            
            if(stateTimer > 0)
                rb.velocity = DeadVelocity;
        }
    }
}