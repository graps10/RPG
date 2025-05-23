using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
  private Animator anim;
  private Rigidbody2D rb;
  private CircleCollider2D cd;
  private Player player;

  private bool canRotate = true;
  private bool isReturning;

  private float freezeTimeDuration;
  private float returnSpeed;

  [Header("Pierce Info")]
  private int pierceAmount;

  [Header("Bounce Info")]
  private float bounceSpeed;
  private bool isBouncing;
  private int bounceAmount;
  private List<Transform> enemyTarget;
  private int targetIndex;

  [Header("Speed Info")]
  private float maxTravelDistance;
  private float spinDuration;
  private bool isSpinning;
  private float spinTimer;
  private bool wasStopped;

  private float hitTimer;
  private float hitCooldown;
  private float spinDirection;


  void Awake()
  {
    anim = GetComponentInChildren<Animator>();
    rb = GetComponent<Rigidbody2D>();
    cd = GetComponent<CircleCollider2D>();
  }

  void FixedUpdate()
  {
    if (canRotate)
      transform.right = rb.velocity;

    if (isReturning)
    {
      transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
      if (Vector2.Distance(transform.position, player.transform.position) < 1)
        player.CatchTheSword();
    }

    BounceLogic();

    SpinLogic();
  }

  public void SetupSword(Vector2 dir, float gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
  {
    player = _player;
    freezeTimeDuration = _freezeTimeDuration;
    returnSpeed = _returnSpeed;

    rb.velocity = dir;
    rb.gravityScale = gravityScale;

    if (pierceAmount <= 0)
      anim.SetBool("Rotation", true);

    spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

    Invoke("DestroySword", 7);
  }

  public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
  {
    isBouncing = _isBouncing;
    bounceAmount = _amountOfBounces;
    bounceSpeed = _bounceSpeed;

    enemyTarget = new List<Transform>();
  }
  public void SetupPierce(int _pierceAmount)
  {
    pierceAmount = _pierceAmount;
  }
  public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
  {
    isSpinning = _isSpinning;
    maxTravelDistance = _maxTravelDistance;
    spinDuration = _spinDuration;
    hitCooldown = _hitCooldown;
  }

  public void ReturnSword()
  {
    rb.constraints = RigidbodyConstraints2D.FreezeAll;
    transform.parent = null;
    isReturning = true;
  }

  private void DestroySword() => Destroy(gameObject);

  private void StopWhenSpinning()
  {
    wasStopped = true;
    rb.constraints = RigidbodyConstraints2D.FreezePosition;
    spinTimer = spinDuration;
  }

  private void BounceLogic()
  {
    if (isBouncing && enemyTarget.Count > 0)
    {
      // if (enemyTarget[targetIndex] == null) // new
      // {
      //   enemyTarget.RemoveAt(targetIndex);

      //   if (enemyTarget.Count == 0)
      //   {
      //     isBouncing = false;
      //     isReturning = true;
      //     return;
      //   }

      //   if (targetIndex >= enemyTarget.Count)
      //     targetIndex = 0;
      // }

      transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

      if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
      {
        SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

        targetIndex++;
        bounceAmount--;

        if (bounceAmount <= 0)
        {
          isBouncing = false;
          isReturning = true;
        }

        if (targetIndex >= enemyTarget.Count)
          targetIndex = 0;
      }
    }
  }

  private void SpinLogic()
  {
    if (isSpinning)
    {
      if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
      {
        StopWhenSpinning();
      }
      if (wasStopped)
      {
        spinTimer -= Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection,
        transform.position.y), 2.4f * Time.deltaTime);

        if (spinTimer < 0)
        {
          isSpinning = false;
          isReturning = true;
        }

        hitTimer -= Time.deltaTime;

        if (hitTimer < 0)
        {
          hitTimer = hitCooldown;

          Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
          foreach (var hit in colliders)
          {
            if (hit.GetComponent<Enemy>() != null)
              SwordSkillDamage(hit.GetComponent<Enemy>());
          }
        }
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (isReturning) return;

    if (collision.GetComponent<Enemy>() != null)
    {
      Enemy enemy = collision.GetComponent<Enemy>();
      SwordSkillDamage(enemy);
    }

    player.stats.DoDamage(collision.GetComponent<CharacterStats>());

    SetupTargetsForBounce(collision);

    StuckInto(collision);
  }

  private void SwordSkillDamage(Enemy enemy)
  {
    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
    player.stats.DoDamage(enemyStats);
    if (player.skill.sword.timeStopUnlocked)
      enemy.FreezeTimeFor(freezeTimeDuration);

    if (player.skill.sword.volnerableUnlocked)
      enemyStats.MakeVulnerableFor(freezeTimeDuration);

    ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

    if (equipedAmulet != null)
      equipedAmulet.Effect(enemy.transform);
  }

  private void SetupTargetsForBounce(Collider2D collision)
  {
    if (collision.GetComponent<Enemy>() != null)
    {
      if (isBouncing && enemyTarget.Count <= 0)
      {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach (var hit in colliders)
        {
          if (hit.GetComponent<Enemy>() != null)
          {
            enemyTarget.Add(hit.transform);
          }
        }
      }
    }
  }

  private void StuckInto(Collider2D collision)
  {
    if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
    {
      pierceAmount--;
      return;
    }

    if (isSpinning)
    {
      StopWhenSpinning();
      return;
    }

    canRotate = false;
    cd.enabled = false;

    rb.isKinematic = true;
    rb.constraints = RigidbodyConstraints2D.FreezeAll;

    GetComponentInChildren<ParticleSystem>().Play();

    if (isBouncing && enemyTarget.Count > 0) return;

    anim.SetBool("Rotation", false);
    transform.parent = collision.transform;
  }
}
