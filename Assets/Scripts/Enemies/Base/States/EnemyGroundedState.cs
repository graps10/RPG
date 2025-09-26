using Managers;
using UnityEngine;

namespace Enemies.Base.States
{
    public class EnemyGroundedState<TEnemy> : EnemyState<TEnemy> where TEnemy : Enemy
    {
        public EnemyGroundedState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Update()
        {
            base.Update();

            var playerPos = PlayerManager.Instance.PlayerGameObject.transform.position;
            
            if (enemy.IsPlayerDetected() || 
                Vector2.Distance(enemy.transform.position, playerPos) < enemy.GetAgroDistance())
                stateMachine.ChangeState(enemy.BattleState);
        }
    }
}