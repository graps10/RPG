using Core;
using Enemies.Base;
using UnityEngine;

namespace Enemies.Archer
{
    public class ArcherJumpState : EnemyState<EnemyArcher>
    {
        public ArcherJumpState(EnemyArcher enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            enemy.Rb.velocity = new Vector2(enemy.GetJumpVelocity().x * -enemy.FacingDir, enemy.GetJumpVelocity().y);
        }

        public override void Update()
        {
            base.Update();

            enemy.Anim.SetFloat(AnimatorHashes.YVelocity, rb.velocity.y);

            if (rb.velocity.y < 0 && enemy.IsGroundDetected())
                stateMachine.ChangeState(enemy.BattleState);
        }
    }
}
