using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherGroundedState : EnemyGroundedState<EnemyArcher>
    {
        public ArcherGroundedState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
