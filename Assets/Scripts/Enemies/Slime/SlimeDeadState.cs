using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Slime
{
    public class SlimeDeadState : EnemyDeadState<EnemySlime>
    {
        public SlimeDeadState(EnemySlime enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
