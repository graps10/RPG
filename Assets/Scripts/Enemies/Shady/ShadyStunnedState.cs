using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Shady
{
    public class ShadyStunnedState : EnemyStunnedState<EnemyShady>
    {
        public ShadyStunnedState(EnemyShady enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}