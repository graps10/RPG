using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Skeleton
{
    public class SkeletonStunnedState : EnemyStunnedState<EnemySkeleton>
    {
        public SkeletonStunnedState(EnemySkeleton enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}

