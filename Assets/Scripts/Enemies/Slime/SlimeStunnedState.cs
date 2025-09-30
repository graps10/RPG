using Core;
using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Slime
{
    public class SlimeStunnedState : EnemyStunnedState<EnemySlime>
    {
        public SlimeStunnedState(EnemySlime enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Update()
        {
            if (enemy.IsGroundDetected())
            {
                enemy.Fx.InvokeCancelColorChange(0);
                enemy.Anim.SetTrigger(AnimatorHashes.StunFold);
                
                enemy.Stats.MakeInvincible(true);
            }
            
            base.Update();
        }
        
        public override void Exit()
        {
            base.Exit();

            enemy.Stats.MakeInvincible(false);
        }
    }
}
