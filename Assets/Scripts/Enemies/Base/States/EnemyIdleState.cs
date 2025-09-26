namespace Enemies.Base.States
{
    public class EnemyIdleState<TEnemy> : EnemyGroundedState<TEnemy> where TEnemy: Enemy
    {
        public EnemyIdleState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Enter()
        {
            base.Enter();

            stateTimer = enemy.GetIdleTime();
        }

        public override void Update()
        {
            base.Update();

            if (stateTimer <= 0)
                stateMachine.ChangeState(enemy.MoveState);
        }
    }
}