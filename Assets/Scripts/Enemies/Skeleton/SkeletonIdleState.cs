using Enemies.Base;
using Enemies.Base.States;
using Managers;

namespace Enemies.Skeleton
{
    public class SkeletonIdleState : EnemyIdleState<EnemySkeleton>
    {
        public SkeletonIdleState(EnemySkeleton enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Exit()
        {
            base.Exit();

            AudioManager.Instance.PlaySFX(22, enemy.transform);
        }
    }
}