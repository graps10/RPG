using System.Collections;
using Core;
using Core.Utilities;
using Enemies.Base;
using Items_and_Inventory;
using Skills;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class CloneSkillController : MonoBehaviour
    {
        private const float Clone_Timer_Reset = -0.1f;
    
        private static readonly Color defaultColor = new(1f, 1f, 1f, 1f);
        private static readonly Vector2 attackAnimRange = new(1, 4);
        private static readonly Vector3 cloneDuplicateOffset = new(0.5f, 0f);
    
        [SerializeField] private float colorLosingSpeed;
    
        [SerializeField] private Transform attackCheck;
        [SerializeField] private float attackCheckRadius;

        [Space]
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private float closestEnemyCheckRadius;
        [SerializeField] private Transform closestEnemy;
    
        private Player.Player _player;
        private SpriteRenderer _sr;
        private Animator _anim;

        private float _cloneTimer;
        private float _attackMultiplier;
        private int _facingDir = 1;

        private bool _canDuplicateClone;
        private float _chanceToDuplicate;
    
        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();

            StartCoroutine(FaceClosestTarget());
        }

        private void Update()
        {
            _cloneTimer -= Time.deltaTime;

            if (_cloneTimer < 0)
            {
                _sr.color = new Color(
                    defaultColor.r,
                    defaultColor.g,
                    defaultColor.b,
                    _sr.color.a - (Time.deltaTime * colorLosingSpeed)
                );

                if (_sr.color.a <= 0)
                    Destroy(gameObject);
            }
        }

        public void SetupClone(Transform newTransform, float cloneDuration, bool canAttack, 
            Vector3 offset, bool _canDuplicate, float chanceToDuplicate, Player.Player player, float attackMultiplier)
        {
            if (canAttack)
                _anim.SetInteger(AnimatorHashes.AttackNumber, Random.Range((int)attackAnimRange.x, (int)attackAnimRange.y));

            _attackMultiplier = attackMultiplier;
            _player = player;
            transform.position = newTransform.position + offset;
            _cloneTimer = cloneDuration;

            _canDuplicateClone = _canDuplicate;
            _chanceToDuplicate = chanceToDuplicate;
        }

        private void AnimationTrigger()
        {
            _cloneTimer = Clone_Timer_Reset;
        }
    
        private void AttackTrigger()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                {
                    //player.stats.DoDamage(hit.GetComponent<CharacterStats>());

                    hit.GetComponent<Entity.Entity>().SetupKnockbackDir(transform);

                    PlayerStats playerStats = _player.GetComponent<PlayerStats>();
                    EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                    playerStats.CloneDoDamage(enemyStats, _attackMultiplier);

                    if (_player.Skill.Clone.CanApplyOnHitEffect())
                    {
                        ItemData_Equipment weaponData = Inventory.Instance.GetEquippedItem(EquipmentType.Weapon);

                        if (weaponData != null)
                            weaponData.Effect(hit.transform);
                    }

                    if (_canDuplicateClone)
                    {
                        if (Random.Range(0, 100) < _chanceToDuplicate)
                        {
                            SkillManager.Instance.Clone.CreateClone(hit.transform, 
                                new Vector3(cloneDuplicateOffset.x * _facingDir, cloneDuplicateOffset.y));
                        }
                    }
                }
            }
        }

        private IEnumerator FaceClosestTarget()
        {
            yield return null;

            FindClosestEnemy();

            if (closestEnemy != null)
            {
                if (transform.position.x > closestEnemy.position.x)
                {
                    _facingDir = -1;
                    transform.Rotate(TransformUtils.FlipAngle);
                }
            }
        }

        private void FindClosestEnemy()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, closestEnemyCheckRadius, whatIsEnemy);

            float closestDistance = Mathf.Infinity;

            foreach (var hit in colliders)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, closestEnemyCheckRadius);
        }
    }
}
