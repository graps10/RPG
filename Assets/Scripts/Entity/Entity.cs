using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    [Header("KnockBack Info")]
    [SerializeField] protected Vector2 knockbackPower = new Vector2(7, 12);
    [SerializeField] protected Vector2 knockbackOffset = new Vector2(0.5f, 2f);
    [SerializeField] protected float knockbackDuration = 0.07f;
    protected bool isKnocked;

    [Header("Collisiion Info")]
    public Transform attackCheck;
    public float attackCheckRadius;

    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int knockbackDir { get; private set; }
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public Action onFlipped;


    protected virtual void Awake()
    {
        stats = GetComponent<CharacterStats>();
    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercantage, float _slowDuration)
    {

    }

    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;
    }

    public virtual void CancelKnockBack() => isKnocked = false;

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public void SetupKnockbackPower(Vector2 _knockbackPower) => knockbackPower = _knockbackPower;

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        float xOffset = UnityEngine.Random.Range(knockbackOffset.x, knockbackOffset.y);

        // if(knockbackPower.x > 0 || knockbackPower.y > 0) This makes player immune to freeze effect when he takes hit

        rb.velocity = new Vector2((knockbackPower.x + xOffset) * knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    public virtual void DamageImpact() => StartCoroutine("HitKnockback");

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked) return;

        rb.velocity = new Vector2(0, 0);
    }
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked) return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip

    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        onFlipped?.Invoke();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion
    public virtual void Die()
    {

    }
}
