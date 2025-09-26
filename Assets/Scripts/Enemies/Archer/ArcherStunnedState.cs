using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherStunnedState : EnemyStunnedState<EnemyArcher>
    {
        public ArcherStunnedState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
