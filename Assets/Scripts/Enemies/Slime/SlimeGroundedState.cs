using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Slime
{
    public class SlimeGroundedState : EnemyGroundedState<EnemySlime>
    {
        public SlimeGroundedState(EnemySlime enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
