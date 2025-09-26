using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Slime
{
    public class SlimeIdleState : EnemyIdleState<EnemySlime>
    {
        public SlimeIdleState(EnemySlime enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
