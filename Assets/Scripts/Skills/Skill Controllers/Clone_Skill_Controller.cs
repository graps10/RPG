using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLosingSpeed;

    private float cloneTimer;

    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = 0.8f;

    private Transform closestEnemy;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer < 0)
        {
            sr.color = new Color(1,1,1, sr.color.a - (Time.deltaTime * colorLosingSpeed));
            
            if(sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform, float cloneDuration, bool canAttack)
    {
        if(canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1,4));
        transform.position = _newTransform.position;
        cloneTimer = cloneDuration;

        FaceClosestTarget();
    }

    private void AnimatinTrigger()
    {
        cloneTimer = -0.1f;
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().Damage();
            }
        }
    }

    private void FaceClosestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if(distanceToEnemy < closestDistance)
                {   
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
                    
            }
        }

        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)
                transform.Rotate(0, 180,0);
        }
    }
}
