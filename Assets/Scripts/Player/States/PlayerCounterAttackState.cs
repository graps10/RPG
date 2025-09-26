using Controllers;
using Core;
using Core.Interfaces;
using Enemies.Base;
using UnityEngine;

namespace Player.States
{
    public class PlayerCounterAttackState : PlayerState
    {
        private const float Counter_Attack_Reset_Time = 10f;
        
        private bool _canCreateClone;
        public PlayerCounterAttackState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            _canCreateClone = true;

            stateTimer = player.GetCounterAttackDuration();
            player.Anim.SetBool(AnimatorHashes.SuccessfulCounterAttack, false);
        }
        
        public override void Update()
        {
            base.Update();

            player.SetZeroVelocity();

            Collider2D[] colliders = Physics2D.OverlapCircleAll(player.GetAttackCheckTransform().position, player.GetAttackCheckRadius());
            foreach (var hit in colliders)
            {
                if (hit.GetComponent<ArrowController>() != null)
                {
                    hit.GetComponent<ArrowController>().FlipArrow();
                    SuccessfulCounterAttack();
                }

                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy == null) continue;
                if (enemy is not IStunnable stunnable || !stunnable.CanBeStunned()) continue;
                
                SuccessfulCounterAttack();

                player.Skill.Parry.UseSkill(); // to restore health on parry

                if (_canCreateClone)
                {
                    _canCreateClone = false;
                    player.Skill.Parry.MakeMirageOnParry(hit.transform);
                }
            }

            if (stateTimer < 0 || triggerCalled)
                stateMachine.ChangeState(player.IdleState);
        }

        private void SuccessfulCounterAttack()
        {
            stateTimer = Counter_Attack_Reset_Time; // any value bigger than 1
            player.Anim.SetBool(AnimatorHashes.SuccessfulCounterAttack, true);
        }
    }
}
