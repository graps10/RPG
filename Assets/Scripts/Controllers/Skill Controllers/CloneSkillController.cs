using System.Collections;
using Core;
using Core.ObjectPool;
using Core.ObjectPool.Configs.Controllers;
using Core.Utilities;
using Enemies.Base;
using Items_and_Inventory;
using Skills;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class CloneSkillController : PooledObject
    {
        private const float Clone_Timer_Reset = -0.1f;
        
        [SerializeField] private Transform attackCheck;
        [SerializeField] private float attackCheckRadius;
    
        private ClonePoolConfig _config;
        private Player.Player _player;
        private PlayerStats _playerStats;
        private SpriteRenderer _sr;
        private Animator _anim;

        private Transform _closestEnemy;
        private float _cloneTimer;
        private float _attackMultiplier;
        private int _facingDir = 1;

        private bool _canDuplicateClone;
        private float _chanceToDuplicate;
    
        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();
        }
        
        public void SetupClone(Transform newTransform, float cloneDuration, bool canAttack, 
            Vector3 offset, bool _canDuplicate, float chanceToDuplicate, Player.Player player, float attackMultiplier, ClonePoolConfig config)
        {
            _config = config;
            _attackMultiplier = attackMultiplier;
            _player = player;
            if(_playerStats == null) // TODO: Can do better
                _playerStats = player.GetComponent<PlayerStats>();
            
            _cloneTimer = cloneDuration;
            
            _canDuplicateClone = _canDuplicate;
            _chanceToDuplicate = chanceToDuplicate;
            
            transform.position = newTransform.position + offset;

            if (canAttack)
                _anim.SetInteger(AnimatorHashes.AttackNumber, Random.Range((int)_config.AttackAnimRange.x, (int)_config.AttackAnimRange.y));
            
            StartCoroutine(FaceClosestTarget());
        }
        
        public override void ReturnToPool()
        {
            if (_sr != null) 
                _sr.color = new Color(1f, 1f, 1f, 1f);
                
            transform.rotation = Quaternion.identity;
            _facingDir = 1;
            _closestEnemy = null;
            
            if (_anim != null)
            {
                _anim.Rebind();
                _anim.Update(0f);
            }
            
            base.ReturnToPool();
        }

        private void Update()
        {
            _cloneTimer -= Time.deltaTime;

            if (_cloneTimer < 0)
            {
                _sr.color = new Color(
                    1f, 1f, 1f,
                    _sr.color.a - (Time.deltaTime * _config.ColorLosingSpeed)
                );

                if (_sr.color.a <= 0)
                    ReturnToPool();
            }
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
                if (hit.TryGetComponent(out Enemy enemy))
                {
                    enemy.SetupKnockbackDir(transform);

                    //player.stats.DoDamage(hit.GetComponent<CharacterStats>());
                    if (hit.TryGetComponent(out EnemyStats enemyStats))
                        _playerStats.CloneDoDamage(enemyStats, _attackMultiplier);

                    if (SkillManager.Instance.Clone.CanApplyOnHitEffect())
                    {
                        ItemData_Equipment weaponData = Inventory.Instance.GetEquippedItem(EquipmentType.Weapon);
                        if (weaponData != null)
                            weaponData.Effect(hit.transform);
                    }

                    if (_canDuplicateClone && Random.Range(0, 100) < _chanceToDuplicate)
                    {
                        SkillManager.Instance.Clone.CreateClone(hit.transform, 
                            new Vector3(_config.CloneDuplicateOffset.x * _facingDir, _config.CloneDuplicateOffset.y));
                    }
                }
            }
        }

        private IEnumerator FaceClosestTarget()
        {
            yield return null;

            FindClosestEnemy();

            if (_closestEnemy != null)
            {
                if (transform.position.x > _closestEnemy.position.x)
                {
                    _facingDir = -1;
                    transform.Rotate(TransformUtils.FlipAngle);
                }
            }
        }

        private void FindClosestEnemy()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 
                _config.ClosestEnemyCheckRadius, _config.WhatIsEnemy);

            float closestDistance = Mathf.Infinity;

            foreach (var hit in colliders)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    _closestEnemy = hit.transform;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_config == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _config.ClosestEnemyCheckRadius);
        }
    }
}
