using System;
using System.Collections;
using Core.Utilities;
using Stats;
using UnityEngine;

namespace Entity
{
    public class Entity : MonoBehaviour
    {
        [Header("KnockBack Info")]
        [SerializeField] protected Vector2 knockbackPower = new(7, 12);
        [SerializeField] protected Vector2 knockbackOffset = new(0.5f, 2f);
        [SerializeField] protected float knockbackDuration = 0.07f;
    
        [Header("Collision Info")]
        [SerializeField] protected Transform attackCheck;
        [SerializeField] protected float attackCheckRadius;

        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected float groundCheckDistance;
        [SerializeField] protected Transform wallCheck;
        [SerializeField] protected float wallCheckDistance;
        [SerializeField] protected LayerMask whatIsGround;
    
        #region Components
    
        public Animator Anim { get; private set; }
        public Rigidbody2D Rb { get; private set; }
        public SpriteRenderer Sr { get; private set; }
        public CharacterStats Stats { get; private set; }
        public CapsuleCollider2D Cd { get; private set; }
    
        #endregion

        public int KnockbackDir { get; private set; }
        public int FacingDir { get; private set; } = 1;
    
        public event Action OnFlipped;
    
        protected bool isKnocked;
        protected bool facingRight = true;
    
        protected virtual void Awake()
        {
            Sr = GetComponentInChildren<SpriteRenderer>();
            Anim = GetComponentInChildren<Animator>();
            Rb = GetComponent<Rigidbody2D>();
            Cd = GetComponent<CapsuleCollider2D>();
            
            Stats = GetComponent<CharacterStats>();
        }

        protected virtual void Start() { }

        protected virtual void Update() { }

        public virtual void SlowEntityBy(float slowPercantage, float slowDuration) { }

        public virtual void SetupKnockbackDir(Transform damageDirection)
        {
            if (damageDirection.position.x > transform.position.x)
                KnockbackDir = -1;
            else if (damageDirection.position.x < transform.position.x)
                KnockbackDir = 1;
        }

        public virtual void CancelKnockBack() => isKnocked = false;

        protected virtual void ReturnDefaultSpeed()
        {
            Anim.speed = 1;
        }

        public void SetupKnockbackPower(Vector2 _knockbackPower) => knockbackPower = _knockbackPower;

        public Transform GetAttackCheckTransform() => attackCheck;
        public float GetAttackCheckRadius() => attackCheckRadius;

        protected virtual IEnumerator HitKnockback()
        {
            isKnocked = true;

            float xOffset = UnityEngine.Random.Range(knockbackOffset.x, knockbackOffset.y);

            // if(knockbackPower.x > 0 || knockbackPower.y > 0) This makes player immune to freeze effect when he takes hit

            Rb.velocity = new Vector2((knockbackPower.x + xOffset) * KnockbackDir, knockbackPower.y);

            yield return new WaitForSeconds(knockbackDuration);

            isKnocked = false;
            SetupZeroKnockbackPower();
        }

        public virtual void DamageImpact() => StartCoroutine(nameof(HitKnockback));

        protected virtual void SetupZeroKnockbackPower() { }
        
        public virtual void Die() { }

        #region Velocity
    
        public void SetZeroVelocity()
        {
            if (isKnocked) return;

            Rb.velocity = Vector2.zero;
        }
        
        public void SetVelocity(float xVelocity, float yVelocity)
        {
            if (isKnocked) return;

            Rb.velocity = new Vector2(xVelocity, yVelocity);
            FlipController(xVelocity);
        }
    
        #endregion

        #region Flip

        public virtual void Flip()
        {
            FacingDir *= -1;
            facingRight = !facingRight;
            transform.Rotate(TransformUtils.FlipAngle);

            OnFlipped?.Invoke();
        }

        protected virtual void FlipController(float x)
        {
            if (x > 0 && !facingRight || x < 0 && facingRight)
                Flip();
        }
    
        #endregion
        
        #region Collision
    
        public virtual bool IsGroundDetected() 
            => Physics2D.Raycast(groundCheck.position, 
                Vector2.down, groundCheckDistance, whatIsGround);

        public virtual bool IsWallDetected() 
            => Physics2D.Raycast(wallCheck.position, 
                Vector2.right * FacingDir, wallCheckDistance, whatIsGround);

        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawLine(groundCheck.position, 
                new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
            Gizmos.DrawLine(wallCheck.position, 
                new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

            Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
        }
        
        #endregion
    }
}
