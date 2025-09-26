using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.DeathBringer
{
    public class DeathBringerAttackState : EnemyAttackState<EnemyDeathBringer>
    {
        public DeathBringerAttackState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            enemy.IncreaseChangeToTeleport(5);
        }
        
        public override void Update()
        {
            base.Update();

            enemy.SetZeroVelocity();

            if (!triggerCalled) return;

            stateMachine.ChangeState(enemy.CanTeleport() ? enemy.TeleportState : enemy.BattleState);
        }
    }
}