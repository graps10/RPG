using Enemies.Base;
using Managers;
using UnityEngine;

namespace Enemies.DeathBringer
{
    public class DeathBringerIdleState : EnemyState<EnemyDeathBringer>
    {
        public DeathBringerIdleState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            stateTimer = enemy.GetIdleTime();
        }

        public override void Update()
        {
            base.Update();

            var playerPos = PlayerManager.Instance.PlayerGameObject.transform.position;
            if (Vector2.Distance(playerPos, enemy.transform.position) < enemy.GetAgroDistance())
                enemy.SetBossFightBegin(true);

            if (stateTimer < 0 && enemy.IsBossFightBegun())
                stateMachine.ChangeState(enemy.BattleState);
        }
    }
}