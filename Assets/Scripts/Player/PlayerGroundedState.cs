using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.R) && IsBlackHoleUnlocked())
        {
            if (player.skill.blackHole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("Cooldown");
                return;
            }

            stateMachine.ChangeState(player.blackHoleState);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && IsSwordUnlocked())
            stateMachine.ChangeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.Q) && HasSword() && IsParryUnlocked())
        {
            if (!player.skill.parry.CanUseSkill())
                return;

            stateMachine.ChangeState(player.counterAttack);
        }


        if (Input.GetKeyDown(KeyCode.Mouse0) && HasSword())
            stateMachine.ChangeState(player.primaryAttack);


        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);


        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
    private bool HasSword()
    {
        if (!player.sword)
        {
            return true;
        }

        return false;
    }

    private bool IsSwordUnlocked()
    {
        if (!player.skill.sword.swordUnlocked)
            return false;

        return true;
    }

    private bool IsParryUnlocked()
    {
        if (!player.skill.parry.parryUnlocked)
            return false;

        return true;
    }

    private bool IsBlackHoleUnlocked()
    {
        if (!player.skill.blackHole.blackHoleUnlocked)
            return false;

        return true;
    }
}
