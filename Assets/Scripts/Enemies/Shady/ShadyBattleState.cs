using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Shady
{
    public class ShadyBattleState : EnemyBattleState<EnemyShady>
    {
        private float _defaultSpeed;
        
        public ShadyBattleState(EnemyShady enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            
            _defaultSpeed = enemy.GetMoveSpeed();
            enemy.SetMoveSpeed(enemy.GetBattleStateMoveSpeed());
        }
        
        public override void Update()
        {
            base.Update();

            HandleBattleBehavior();
            CalculateMoveDirection();

            enemy.SetVelocity(enemy.GetMoveSpeed() * moveDir, rb.velocity.y);
        }

        public override void Exit()
        {
            base.Exit();

            enemy.SetMoveSpeed(_defaultSpeed);
        }

        protected override void HandleBattleBehavior()
        {
            if (enemy.IsPlayerDetected())
            {
                stateTimer = enemy.GetBattleTime();

                if (enemy.IsPlayerDetected().distance < enemy.GetAttackDistance())
                    enemy.Stats.KillEntity(); // this enters dead state which triggers explosion + drop items and souls
            }
            else
            {
                if (!flippedOnce)
                {
                    flippedOnce = true;
                    enemy.Flip();
                }

                if (CanReturnToIdle())
                    stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}
