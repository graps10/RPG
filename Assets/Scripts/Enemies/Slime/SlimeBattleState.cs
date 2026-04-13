using Enemies.Base;
using Enemies.Base.States;

namespace Enemies.Slime
{
    public class SlimeBattleState : EnemyBattleState<EnemySlime>
    {
        public SlimeBattleState(EnemySlime enemy, EnemyStateMachine stateMachine, int animBoolName) :
            base(enemy, stateMachine, animBoolName) { }
    }
}
