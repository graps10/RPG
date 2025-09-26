using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Shady
{
    public class ShadyGroundedState : EnemyGroundedState<EnemyShady>
    {
        public ShadyGroundedState(EnemyShady enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}