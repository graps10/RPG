using UnityEngine;

namespace Enemies.Base
{
    public abstract class EnemyState<TEnemy> : EnemyState where TEnemy : Enemy
    {
        protected new TEnemy enemy;

        public EnemyState(TEnemy enemy, EnemyStateMachine stateMachine, int animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            this.enemy = enemy;
        }
    }
    
    public abstract class EnemyState
    {
        protected const float COLOR_BLINK_REPEAT_RATE = 0.1f;
        
        protected EnemyStateMachine stateMachine;
        protected Enemy enemy;

        protected Rigidbody2D rb;
    
        protected float stateTimer;
        protected bool triggerCalled;
    
        private int _animBoolName;

        public EnemyState(Enemy enemy, EnemyStateMachine stateMachine, int animBoolName)
        {
            this.enemy = enemy;
            this.stateMachine = stateMachine;
            _animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            enemy.Anim.SetBool(_animBoolName, true);
            rb = enemy.Rb;
            triggerCalled = false;
        }

        public virtual void Update()
        {
            stateTimer -= Time.deltaTime;
        }

        public virtual void Exit()
        {
            enemy.Anim.SetBool(_animBoolName, false);
            enemy.AssignLastAnimName(_animBoolName);
        }

        public void AnimationFinishTrigger()
        {
            triggerCalled = true;
        }
    }
}