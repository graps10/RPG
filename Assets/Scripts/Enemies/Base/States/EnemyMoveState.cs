namespace Enemies.Base.States
{
    public class EnemyMoveState<TEnemy> : EnemyGroundedState<TEnemy> where TEnemy: Enemy
    {
        public EnemyMoveState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }
        
        public override void Update()
        {
            base.Update();

            enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.FacingDir, rb.velocity.y);

            if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
            {
                enemy.Flip();
                stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}