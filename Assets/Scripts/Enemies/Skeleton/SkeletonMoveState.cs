using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Skeleton
{
    public class SkeletonMoveState : EnemyMoveState<EnemySkeleton>
    {
        public SkeletonMoveState(EnemySkeleton enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}