using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    #endregion

    [Header("KnockBack Info")]
    [SerializeField] protected Vector2 knockbackPower;
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

    public int facingDir {get; private set; } = 1;
    protected bool facingRight = true;
    

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {

    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackPower.x * -facingDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }

    public virtual void Damage()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");

        // Debug.Log(gameObject.name + " was damaged!");
    }

    #region Velocity
    public void SetZeroVelocity()
    { 
        if(isKnocked) return;

        rb.velocity = new Vector2(0,0);
    } 
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if(isKnocked) return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos() {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip(){
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public virtual void FlipController(float _x){
        if(_x > 0 && !facingRight){
            Flip();
        }else if(_x < 0 && facingRight){
            Flip();
        }
    }
    #endregion
}
