using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private string targetLayerName = "Player";

    [SerializeField] private float xVelocity;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool canMove = true;
    [SerializeField] private bool flipped;

    private CharacterStats myStats;

    void Update()
    {
        if (canMove)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            //collision.GetComponent<CharacterStats>()?.TakeDamage(damage);

            myStats.DoDamage(collision.GetComponent<CharacterStats>());
            StuckInto(collision);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StuckInto(collision);
        }
    }

    public void SetupArrow(CharacterStats _myStats, float _speed)
    {
        myStats = _myStats;
        xVelocity = _speed;
    }

    public void FlipArrow()
    {
        if (flipped)
            return;

        xVelocity *= -1;
        flipped = true;

        transform.Rotate(0, 180, 0);

        targetLayerName = "Enemy";
    }

    private void StuckInto(Collider2D collision)
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<CapsuleCollider2D>().enabled = false;

        canMove = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = collision.transform;

        Destroy(gameObject, Random.Range(5, 7));
    }
}
