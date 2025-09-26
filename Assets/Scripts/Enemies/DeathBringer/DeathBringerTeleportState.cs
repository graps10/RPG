using Enemies.Base;

namespace Enemies.DeathBringer
{
    public class DeathBringerTeleportState : EnemyState<EnemyDeathBringer>
    {
        public DeathBringerTeleportState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            enemy.Stats.MakeInvincible(true);
        }

        public override void Update()
        {
            base.Update();

            if (!triggerCalled) return;

            stateMachine.ChangeState(enemy.CanCastSpell() ? enemy.SpellCastState : enemy.BattleState);
        }

        public override void Exit()
        {
            base.Exit();

            enemy.Stats.MakeInvincible(false);
        }
    }
}