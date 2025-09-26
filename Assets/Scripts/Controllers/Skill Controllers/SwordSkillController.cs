using System.Collections.Generic;
using Core;
using Enemies.Base;
using Items_and_Inventory;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
  public class SwordSkillController : MonoBehaviour
  {
    private const float Catch_Distance_Threshold = 1f;
    private const float Bounce_Distance_Threshold = 0.1f;
    
    private const float Spin_Move_Speed = 2.4f;
    private const float Spin_Hit_Radius = 1f;
    
    private const float Bounce_Target_Search_Radius = 10f;
    
    private const float Destroy_Sword_Delay = 7f;
    
    private static readonly Vector2 spinDirectionRange = new(-1f, 1f);
    
    private Animator _anim;
    private Rigidbody2D _rb;
    private CircleCollider2D _cd;
    private Player.Player _player;

    private bool _canRotate = true;
    private bool _isReturning;

    private float _freezeTimeDuration;
    private float _returnSpeed;

    // Pierce Info
    private int _pierceAmount;

    // Bounce Info
    private float _bounceSpeed;
    private bool _isBouncing;
    private int _bounceAmount;
    private List<Transform> _enemyTarget;
    private int _targetIndex;

    // Speed Info
    private float _maxTravelDistance;
    private float _spinDuration;
    private bool _isSpinning;
    private float _spinTimer;
    private bool _wasStopped;

    private float _hitTimer;
    private float _hitCooldown;
    private float _spinDirection;

    private void Awake()
    {
      _anim = GetComponentInChildren<Animator>();
      _rb = GetComponent<Rigidbody2D>();
      _cd = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
      if (_canRotate)
        transform.right = _rb.velocity;

      if (_isReturning)
      {
        transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, 
          _returnSpeed * Time.deltaTime);
        
        if (Vector2.Distance(transform.position, _player.transform.position) < Catch_Distance_Threshold)
          _player.CatchTheSword();
      }

      BounceLogic();

      SpinLogic();
    }

    public void SetupSword(Vector2 dir, float gravityScale, Player.Player player, float freezeTimeDuration, float returnSpeed)
    {
      _player = player;
      _freezeTimeDuration = freezeTimeDuration;
      _returnSpeed = returnSpeed;

      _rb.velocity = dir;
      _rb.gravityScale = gravityScale;

      if (_pierceAmount <= 0)
        _anim.SetBool(AnimatorHashes.Rotation, true);

      _spinDirection = Mathf.Clamp(_rb.velocity.x, spinDirectionRange.x, spinDirectionRange.y);

      Invoke(nameof(DestroySword), Destroy_Sword_Delay);
    }

    public void SetupBounce(bool isBouncing, int amountOfBounces, float bounceSpeed)
    {
      _isBouncing = isBouncing;
      _bounceAmount = amountOfBounces;
      _bounceSpeed = bounceSpeed;

      _enemyTarget = new List<Transform>();
    }
    public void SetupPierce(int pierceAmount)
    {
      _pierceAmount = pierceAmount;
    }
    public void SetupSpin(bool isSpinning, float maxTravelDistance, float spinDuration, float hitCooldown)
    {
      _isSpinning = isSpinning;
      _maxTravelDistance = maxTravelDistance;
      _spinDuration = spinDuration;
      _hitCooldown = hitCooldown;
    }

    public void ReturnSword()
    {
      _rb.constraints = RigidbodyConstraints2D.FreezeAll;
      transform.parent = null;
      _isReturning = true;
    }

    private void DestroySword() => Destroy(gameObject);

    private void StopWhenSpinning()
    {
      _wasStopped = true;
      _rb.constraints = RigidbodyConstraints2D.FreezePosition;
      _spinTimer = _spinDuration;
    }

    private void BounceLogic()
    {
      if (_isBouncing && _enemyTarget.Count > 0)
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

        transform.position = Vector2.MoveTowards(transform.position, 
          _enemyTarget[_targetIndex].position, _bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _enemyTarget[_targetIndex].position) < Bounce_Distance_Threshold)
        {
          SwordSkillDamage(_enemyTarget[_targetIndex].GetComponent<Enemy>());

          _targetIndex++;
          _bounceAmount--;

          if (_bounceAmount <= 0)
          {
            _isBouncing = false;
            _isReturning = true;
          }

          if (_targetIndex >= _enemyTarget.Count)
            _targetIndex = 0;
        }
      }
    }

    private void SpinLogic()
    {
      if (_isSpinning)
      {
        if (Vector2.Distance(_player.transform.position, transform.position) > _maxTravelDistance && !_wasStopped)
        {
          StopWhenSpinning();
        }
        if (_wasStopped)
        {
          _spinTimer -= Time.deltaTime;

          transform.position = Vector2.MoveTowards(transform.position, 
            new Vector2(transform.position.x + _spinDirection,
            transform.position.y), Spin_Move_Speed * Time.deltaTime);

          if (_spinTimer < 0)
          {
            _isSpinning = false;
            _isReturning = true;
          }

          _hitTimer -= Time.deltaTime;

          if (_hitTimer < 0)
          {
            _hitTimer = _hitCooldown;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Spin_Hit_Radius);
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
      if (_isReturning) return;

      if (collision.GetComponent<Enemy>() != null)
      {
        Enemy enemy = collision.GetComponent<Enemy>();
        SwordSkillDamage(enemy);
      }

      _player.Stats.DoDamage(collision.GetComponent<CharacterStats>());

      SetupTargetsForBounce(collision);

      StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
      EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
      _player.Stats.DoDamage(enemyStats);
      
      if (_player.Skill.Sword.IsTimeStopUnlocked())
        enemy.FreezeTimeFor(_freezeTimeDuration);

      if (_player.Skill.Sword.IsVulnerableUnlocked())
        enemyStats.MakeVulnerableFor(_freezeTimeDuration);

      ItemData_Equipment equippedAmulet = Inventory.Instance.GetEquipment(EquipmentType.Amulet);

      if (equippedAmulet != null)
        equippedAmulet.Effect(enemy.transform);
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
      if (collision.GetComponent<Enemy>() != null)
      {
        if (_isBouncing && _enemyTarget.Count <= 0)
        {
          Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Bounce_Target_Search_Radius);
          foreach (var hit in colliders)
          {
            if (hit.GetComponent<Enemy>() != null)
            {
              _enemyTarget.Add(hit.transform);
            }
          }
        }
      }
    }

    private void StuckInto(Collider2D collision)
    {
      if (_pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
      {
        _pierceAmount--;
        return;
      }

      if (_isSpinning)
      {
        StopWhenSpinning();
        return;
      }

      _canRotate = false;
      _cd.enabled = false;

      _rb.isKinematic = true;
      _rb.constraints = RigidbodyConstraints2D.FreezeAll;

      GetComponentInChildren<ParticleSystem>().Play();

      if (_isBouncing && _enemyTarget.Count > 0) return;

      _anim.SetBool(AnimatorHashes.Rotation, false);
      transform.parent = collision.transform;
    }
  }
}
