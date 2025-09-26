using Core;
using UnityEngine;

namespace Player
{
    public class PlayerState
    {
        protected Rigidbody2D rb;
        
        protected PlayerStateMachine stateMachine;
        protected Player player;
        
        protected float xInput; 
        protected float yInput; 
        
        protected float stateTimer;
        protected bool triggerCalled;
        
        private int _animBoolName;

        public PlayerState(Player player, PlayerStateMachine stateMachine, int animBoolName)
        {
            this.player = player;
            this.stateMachine = stateMachine;
            _animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            player.Anim.SetBool(_animBoolName, true);
            rb = player.Rb;
            triggerCalled = false;
        }

        public virtual void Update()
        {
            stateTimer -= Time.deltaTime;

            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
            
            player.Anim.SetFloat(AnimatorHashes.YVelocity, rb.velocity.y);
        }

        public virtual void Exit()
        {
            player.Anim.SetBool(_animBoolName, false);
        }

        public void AnimationFinishTrigger()
        {
            triggerCalled = true;
        }
    }
}
