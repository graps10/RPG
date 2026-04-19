using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.DeathBringer
{
    public class DeathBringerAttackState : EnemyAttackState<EnemyDeathBringer>
    {
        private const int Teleport_Increase_Chance_Rate = 5;
            
        public DeathBringerAttackState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            enemy.IncreaseTheChanceToTeleport(Teleport_Increase_Chance_Rate);
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