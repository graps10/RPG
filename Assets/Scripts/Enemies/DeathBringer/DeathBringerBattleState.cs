using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.DeathBringer
{
    public class DeathBringerBattleState : EnemyBattleState<EnemyDeathBringer>
    {
        public DeathBringerBattleState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        protected override void HandleBattleBehavior()
        {
            if (moveDir != 0 && moveDir != enemy.FacingDir)
                enemy.Flip();

            var hit = enemy.IsPlayerDetected();

            if (hit)
            {
                stateTimer = enemy.GetBattleTime();

                if (hit.distance < enemy.GetAttackDistance())
                {
                    if (enemy.CanAttack())
                        stateMachine.ChangeState(enemy.AttackState);
                }
            }
        }
    }
}
