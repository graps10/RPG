using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Skeleton
{
    public class SkeletonAttackState : EnemyAttackState<EnemySkeleton>
    {
        public SkeletonAttackState(EnemySkeleton enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
    }
}
