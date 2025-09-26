using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.DeathBringer
{
    public class DeathBringerDeadState : EnemyDeadState<EnemyDeathBringer>
    {
        public DeathBringerDeadState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}