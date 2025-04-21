using UnityEngine;

public class ShadyBattleState : EnemyState
{
    private Transform player;
    private Enemy_Shady enemy;
    protected int moveDir;

    private float defaultSpeed;
    private bool flippedOnce;

    public ShadyBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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

        defaultSpeed = enemy.moveSpeed;
        enemy.moveSpeed = enemy.battleStateMoveSpeed;

        flippedOnce = false;
    }
    public override void Update()
    {
        base.Update();

        HandleBattleBehavior();
        CalculateMoveDirection();

        // if (enemy.IsPlayerDetected())
        // {
        //     if (enemy.IsPlayerDetected().distance < enemy.attackDistance - 0.5f)
        //     {
        //         enemy.anim.SetFloat("xVelocity", 0);
        //         return;
        //     }
        //     else
        //         enemy.anim.SetFloat("xVelocity", moveDir);
        // }

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = defaultSpeed;
    }

    private void HandleBattleBehavior()
    {
        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
                enemy.stats.KillEntity(); // this enters dead state which triggers explosion + drop items and souls
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
}
