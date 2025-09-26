using UnityEngine;

namespace Enemies.Base.States
{
    public class EnemyAttackState<TEnemy> : EnemyState<TEnemy> where TEnemy : Enemy
    {
        public EnemyAttackState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) :
            base(enemy, stateMachine, animBoolName) { }

        public override void Update()
        {
            base.Update();

            enemy.SetZeroVelocity();

            if (triggerCalled)
                stateMachine.ChangeState(enemy.BattleState);
        }

        public override void Exit()
        {
            base.Exit();

            enemy.SetLastTimeAttacked(Time.time);
        }
    }
}