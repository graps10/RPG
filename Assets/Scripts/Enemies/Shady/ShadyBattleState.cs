using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Shady
{
    public class ShadyBattleState : EnemyBattleState<EnemyShady>
    {
        private float _defaultSpeed;
        
        public ShadyBattleState(EnemyShady enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            
            _defaultSpeed = enemy.GetMoveSpeed();
            enemy.SetMoveSpeed(enemy.GetBattleStateMoveSpeed());
        }

        public override void Exit()
        {
            base.Exit();
            enemy.SetMoveSpeed(_defaultSpeed);
        }

        protected override void HandleBattleBehavior()
        {
            if (moveDir != 0 && moveDir != enemy.FacingDir)
                enemy.Flip();

            var hit = enemy.IsPlayerDetected();

            if (hit)
            {
                stateTimer = enemy.GetBattleTime();
                
                if (hit.distance < enemy.GetAttackDistance())
                    enemy.Stats.KillEntity(); 
            }
            else
            {
                if (CanReturnToIdle())
                    stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}
