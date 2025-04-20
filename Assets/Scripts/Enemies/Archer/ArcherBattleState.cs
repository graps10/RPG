using UnityEngine;

public class ArcherBattleState : EnemyState
{
    private Transform player;
    private Enemy_Archer enemy;
    protected int moveDir;

    private bool flippedOnce;

    public ArcherBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);


        stateTimer = enemy.battleTime;
        flippedOnce = false;
    }
    public override void Update()
    {
        base.Update();

        HandleBattleBehavior();
        CalculateMoveDirection();

        if (enemy.IsPlayerDetected())
        {
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance - 0.5f)
            {
                enemy.anim.SetFloat("xVelocity", 0);
                return;
            }
            else
            {
                enemy.anim.SetFloat("xVelocity", moveDir);
                // enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
            }
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void HandleBattleBehavior()
    {
        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.safeDistance)
            {
                if (CanJump())
                    stateMachine.ChangeState(enemy.jumpState);
            }

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (!flippedOnce)
            {
                flippedOnce = true;
                enemy.Flip();
            }

            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }

    private void CalculateMoveDirection()
    {
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;
    }

    private bool CanJump()
    {
        if (enemy.GroundBehind() == false || enemy.WallBehind() == true)
            return false;

        if (Time.time >= enemy.lastTimeJumped + enemy.jumpCooldown)
        {
            enemy.lastTimeJumped = Time.time;
            return true;
        }

        return false;
    }
}
