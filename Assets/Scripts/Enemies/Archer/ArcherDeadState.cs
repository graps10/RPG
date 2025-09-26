using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherDeadState : EnemyDeadState<EnemyArcher>
    {
        public ArcherDeadState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
