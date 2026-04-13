using Enemies.Base;

namespace Enemies.DeathBringer
{
    public class DeathBringerIdleState : EnemyState<EnemyDeathBringer>
    {
        public DeathBringerIdleState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            stateTimer = enemy.GetIdleTime();
        }
    }
}