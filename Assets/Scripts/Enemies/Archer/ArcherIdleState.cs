using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherIdleState : EnemyIdleState<EnemyArcher>
    {
        public ArcherIdleState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
