using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Skeleton
{
    public class SkeletonBattleState : EnemyBattleState<EnemySkeleton>
    {
        public SkeletonBattleState(EnemySkeleton enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Update()
        {
            base.Update();

            HandleBattleBehavior();
            CalculateMoveDirection();

            ChasePlayer();
        }
    }
}
