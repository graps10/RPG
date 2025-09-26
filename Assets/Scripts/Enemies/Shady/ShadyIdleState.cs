using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Shady
{
    public class ShadyIdleState : EnemyIdleState<EnemyShady>
    {
        public ShadyIdleState(EnemyShady enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
