using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Slime
{
    public class SlimeAttackState : EnemyAttackState<EnemySlime>
    {
        public SlimeAttackState(EnemySlime enemy, EnemyStateMachine stateMachine, int animBoolName) :
            base(enemy, stateMachine, animBoolName)
        {
            this.enemy = enemy;
        }
    }
}
