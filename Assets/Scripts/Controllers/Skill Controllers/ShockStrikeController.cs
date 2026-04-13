using System;
using System.Collections;
using Core;
using Core.ObjectPool;
using Core.ObjectPool.Configs.FX;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class ShockStrikeController : PooledObject
    {
        private CharacterStats _targetStats;
        private ShockStrikePoolConfig _config;
        private Animator _anim;
    
        private int _damage;
        private bool _triggered;
        private Quaternion _animInitialRotation;

        private void Awake()
        {
            if(_anim == null)
                _anim = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _animInitialRotation = _anim.transform.rotation;
        }

        public void Setup(int damage, CharacterStats targetStats,  ShockStrikePoolConfig config)
        {
            _damage = damage;
            _targetStats = targetStats;
            _config = config;
        }

        public override void ReturnToPool()
        {
            _triggered = false;
            _targetStats = null;

            transform.position = Vector2.zero;
            transform.localScale = Vector3.one;
            
            _anim.transform.localPosition = Vector3.zero;
            _anim.transform.localRotation = _animInitialRotation;
            
            _anim.Rebind();
            _anim.Update(0f);
            
            base.ReturnToPool();
        }

        private void Update()
        {
            if (!_targetStats || _triggered) 
                return;
        
            transform.position = Vector2.MoveTowards(transform.position, 
                _targetStats.transform.position, _config.Speed * Time.deltaTime);
        
            transform.right = transform.position - _targetStats.transform.position;

            if (Vector2.Distance(transform.position, _targetStats.transform.position) < _config.HitDistanceThreshold)
            {
                _triggered = true;

                _anim.transform.localRotation = Quaternion.identity;
                _anim.transform.localPosition = _config.AnimLocalPosition;

                transform.localRotation = Quaternion.identity;
                transform.localScale = _config.HitScale;

                StartCoroutine(DamageAndSelfDestroy());
                _anim.SetTrigger(AnimatorHashes.Hit);
            }
        }

        private IEnumerator DamageAndSelfDestroy()
        {
            yield return new WaitForSeconds(_config.DamageDelay);
            _targetStats.ApplyShock(true);
            _targetStats.TakeDamage(_damage);
            
            yield return new WaitForSeconds(_config.ReturnDelay);
            ReturnToPool();
        }
    }
}
