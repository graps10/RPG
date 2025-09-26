using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Skeleton
{
    public class SkeletonDeadState : EnemyDeadState<EnemySkeleton>
    {
        public SkeletonDeadState(EnemySkeleton enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
