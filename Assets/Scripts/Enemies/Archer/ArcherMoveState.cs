using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherMoveState : EnemyMoveState<EnemyArcher>
    {
        public ArcherMoveState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
