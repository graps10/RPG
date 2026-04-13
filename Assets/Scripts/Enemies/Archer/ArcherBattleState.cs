using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Archer
{
    public class ArcherBattleState : EnemyBattleState<EnemyArcher>
    {
        public ArcherBattleState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        protected override void HandleBattleBehavior()
        {
            if (moveDir != 0 && moveDir != enemy.FacingDir)
                enemy.Flip();
            
            var hit = enemy.IsPlayerDetected();

            if (hit)
            {
                stateTimer = enemy.GetBattleTime();
                
                if (hit.distance < enemy.GetSafeDistance())
                {
                    if (enemy.CanJump())
                    {
                        stateMachine.ChangeState(enemy.JumpState);
                        return;
                    }
                }
                
                if (hit.distance < enemy.GetAttackDistance())
                {
                    if (enemy.CanAttack())
                        stateMachine.ChangeState(enemy.AttackState);
                }
            }
            else
            {
                if (CanReturnToIdle())
                    stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}
