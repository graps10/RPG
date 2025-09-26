using Core;
using Stats;
using UnityEngine;

namespace Controllers
{
    public class ExplosiveController : MonoBehaviour
    {
        private const float Size_Threshold = 0.5f;
        
        private Animator _anim;
        private CharacterStats _myStats;
        
        private float _growSpeed;
        private float _maxSize;
        private float _explosionRadius;

        private bool _canGrow = true;

        public void SetupExplosive(CharacterStats myStats, float growSpeed, float maxSize, float radius)
        {
            _anim = GetComponent<Animator>();
            
            _myStats = myStats;
            _growSpeed = growSpeed;
            _maxSize = maxSize;
            _explosionRadius = radius;
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
                if (hit.GetComponent<CharacterStats>() != null)
                {
                    hit.GetComponent<Entity.Entity>().SetupKnockbackDir(transform);
                    _myStats.DoDamage(hit.GetComponent<CharacterStats>());
                }
            }
        }

        private void SelfDestroy() => Destroy(gameObject);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}
