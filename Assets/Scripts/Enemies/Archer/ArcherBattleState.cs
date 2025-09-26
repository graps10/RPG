using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherBattleState : EnemyBattleState<EnemyArcher>
    {
        public ArcherBattleState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Update()
        {
            base.Update();

            HandleBattleBehavior();
            CalculateMoveDirection();

            ChasePlayer();
        }

        protected override void HandleBattleBehavior()
        {
            if (enemy.IsPlayerDetected())
            {
                stateTimer = enemy.GetBattleTime();

                if (enemy.IsPlayerDetected().distance < enemy.GetSafeDistance())
                {
                    if (enemy.CanJump())
                        stateMachine.ChangeState(enemy.JumpState);
                }

                if (enemy.IsPlayerDetected().distance < enemy.GetAttackDistance())
                {
                    if (enemy.CanAttack())
                        stateMachine.ChangeState(enemy.AttackState);
                }
            }
            else
            {
                if (!flippedOnce)
                {
                    flippedOnce = true;
                    enemy.Flip();
                }

                if (CanReturnToIdle())
                    stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}
