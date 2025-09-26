using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Slime
{
    public class SlimeMoveState : EnemyMoveState<EnemySlime>
    {
        public SlimeMoveState(EnemySlime enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
