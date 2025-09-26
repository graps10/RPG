using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherAttackState : EnemyAttackState<EnemyArcher>
    {
        public ArcherAttackState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) :
            base(enemy, stateMachine, animBoolName) { }
    }
}
