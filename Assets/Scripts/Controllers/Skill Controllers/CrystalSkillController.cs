using Core;
using Core.ObjectPool;
using Core.ObjectPool.Configs.Controllers;
using Enemies.Base;
using Items_and_Inventory;
using Skills;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class CrystalSkillController : PooledObject
    {
        private Player.Player _player;
        private Animator _anim => GetComponent<Animator>();
        private CircleCollider2D _cd => GetComponent<CircleCollider2D>();
        
        private CrystalPoolConfig _config;
        
        private float _moveSpeed;
        private float _crystalExistTimer;
        
        private bool _canMove;
        private bool _canGrow;
        private bool _canExplode;
        
        private Transform _closestTarget;

        public void SetupCrystal(CrystalPoolConfig config, float crystalDuration, bool canExplode, bool canMove, float moveSpeed,
            Transform closestTarget,  Player.Player player)
        {
            _player = player;
            _config = config;
            _crystalExistTimer = crystalDuration;
            _canExplode = canExplode;
            _canMove = canMove;
            _moveSpeed = moveSpeed;
            _closestTarget = closestTarget;
        }

        public override void ReturnToPool()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            
            _moveSpeed = 0f;
            _crystalExistTimer = 0f;
            
            _canMove = false;
            _canGrow = false;
            _canExplode = false;
            
            _closestTarget = null;
            
            _anim.Rebind();
            _anim.Update(0f);
            
            base.ReturnToPool();
        }

        public void ChooseRandomEnemy()
        {
            float radius = SkillManager.Instance.BlackHole.GetBlackHoleRadius();

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, _config.WhatIsEnemy);
            if (colliders.Length > 0)
                _closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
        }

        private void Update()
        {
            _crystalExistTimer -= Time.deltaTime;

            if (_crystalExistTimer < 0)
                FinishCrystal();

            if (_canMove)
                FaceClosestTarget();

            if (_canGrow)
                transform.localScale = Vector2.Lerp(transform.localScale, 
                    _config.GrowTargetScale, _config.GrowSpeed * Time.deltaTime);
        }

        public void FinishCrystal()
        {
            if (_canExplode)
            {
                _canGrow = true;
                _anim.SetTrigger(AnimatorHashes.Explode);
                _canMove = false;
            }
            else
                ReturnToPool();
        }
        
        public void SelfDestroy() => ReturnToPool();

        private void AnimationExplodeEvent()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _cd.radius);

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                {
                    hit.GetComponent<Entity.Entity>().SetupKnockbackDir(transform);
                    _player.Stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());

                    ItemData_Equipment equippedAmulet = Inventory.Instance.GetEquippedItem(EquipmentType.Amulet);

                    if (equippedAmulet != null)
                        equippedAmulet.Effect(hit.transform);
                }
            }
        }

        private void FaceClosestTarget()
        {
            if (_closestTarget != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, _closestTarget.position, 
                    _moveSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, _closestTarget.position) < _config.ClosestTargetMinDistance)
                    FinishCrystal();
            }
        }
    }
}
