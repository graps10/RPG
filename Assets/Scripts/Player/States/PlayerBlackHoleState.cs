using UnityEngine;

namespace Player.States
{
    public class PlayerBlackHoleState : PlayerState
    {
        private const float Fly_Time = 0.4f;
        
        private static readonly Vector2 flyUpVelocity = new(0, 12);
        private static readonly Vector2 fallVelocity = new(0, -0.1f);
        
        private bool _skillUsed;
        private float _defaultGravity;

        public PlayerBlackHoleState(Player player, PlayerStateMachine stateMachine, int animBoolName) : 
            base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            _defaultGravity = rb.gravityScale;
            _skillUsed = false;
            stateTimer = Fly_Time;
            rb.gravityScale = 0;
        }
        
        public override void Update()
        {
            base.Update();

            if (stateTimer > 0)
                rb.velocity = flyUpVelocity;

            if (stateTimer < 0)
            {
                rb.velocity = fallVelocity;

                if (!_skillUsed)
                {
                    if (player.Skill.BlackHole.CanUseSkill())
                        _skillUsed = true;
                }
            }

            if (player.Skill.BlackHole.SkillCompleted())
                stateMachine.ChangeState(player.AirState);
        }
        
        public override void Exit()
        {
            base.Exit();

            rb.gravityScale = _defaultGravity;
            player.Fx.MakeTransparent(false);
        }
    }
}
