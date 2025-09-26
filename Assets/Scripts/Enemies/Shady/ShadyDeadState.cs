using Enemies.Base;

namespace Enemies.Shady
{
    public class ShadyDeadState : EnemyState<EnemyShady>
    {
        public ShadyDeadState(EnemyShady enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Update()
        {
            base.Update();

            if (triggerCalled)
                enemy.SelfDestroy();
        }
    }
}
