using Core;
using Core.ObjectPool;
using Stats;
using UnityEngine;

namespace Controllers
{
    public class ExplosiveController : PooledObject
    {
        private const float Size_Threshold = 0.5f;
        
        private Animator _anim;
        private CharacterStats _myStats;
        
        private float _growSpeed;
        private float _maxSize;
        private float _explosionRadius;

        private bool _canGrow = true;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }
        
        public void SetupExplosive(CharacterStats myStats, float growSpeed, float maxSize, float radius)
        {
            _myStats = myStats;
            _growSpeed = growSpeed;
            _maxSize = maxSize;
            _explosionRadius = radius;
            
            _canGrow = true;
            transform.localScale = Vector3.zero;
        }
        
        public override void ReturnToPool()
        {
            _canGrow = false;
            _myStats = null;
            
            if (_anim != null)
            {
                _anim.Rebind();
                _anim.Update(0f);
            }
            
            base.ReturnToPool();
        }
        
        private void Update()
        {
            if (_canGrow)
                transform.localScale = Vector2.Lerp(transform.localScale, 
                    new Vector2(_maxSize, _maxSize), _growSpeed * Time.deltaTime);

            if (_maxSize - transform.localScale.x < Size_Threshold)
            {
                _canGrow = false;
                _anim.SetTrigger(AnimatorHashes.Explode);
            }
        }

        private void AnimationExplodeEvent()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent(out CharacterStats targetStats))
                {
                    if (hit.TryGetComponent(out Entity.Entity entity))
                        entity.SetupKnockbackDir(transform);
                    
                    _myStats.DoDamage(targetStats);
                }
            }
        }

        private void SelfDestroy() => ReturnToPool();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}
