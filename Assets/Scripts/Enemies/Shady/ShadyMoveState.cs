using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Shady
{
    public class ShadyMoveState : EnemyMoveState<EnemyShady>
    {
        public ShadyMoveState(EnemyShady enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
