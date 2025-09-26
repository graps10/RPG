using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.DeathBringer
{
    public class DeathBringerBattleState : EnemyBattleState<EnemyDeathBringer>
    {
        public DeathBringerBattleState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Update()
        {
            base.Update();

            HandleBattleBehavior();
            CalculateMoveDirection();

            ChasePlayer();
        }

        protected override void HandleBattleBehavior()
        {
            if (enemy.IsPlayerDetected())
            {
                stateTimer = enemy.GetBattleTime();

                if (enemy.IsPlayerDetected().distance < enemy.GetAttackDistance())
                {
                    if (enemy.CanAttack())
                        stateMachine.ChangeState(enemy.AttackState);
                }
            }
            else
            {
                if (!flippedOnce)
                {
                    flippedOnce = true;
                    enemy.Flip();
                }
            }
        }
    }
}
