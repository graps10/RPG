using Core;
using Core.ObjectPool;
using Managers;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class ShockStrikeController : MonoBehaviour, IPooledObject
    {
        private const float Hit_Distance_Threshold = 0.1f;
        private const float Damage_Delay = 0.2f;
        private const float Return_Delay = 0.4f;
    
        private static readonly Vector2 animLocalPosition = new(0f, 0.5f);
        private static readonly Vector2 hitScale = new(3f, 3f);
        
        [SerializeField] private float speed;
    
        private CharacterStats _targetStats;
        private Animator _anim;
    
        private int _damage;
        private bool _triggered;

        private void Awake()
        {
            _anim = GetComponentInChildren<Animator>();
        }

        public void Setup(int damage, CharacterStats targetStats)
        {
            _damage = damage;
            _targetStats = targetStats;
        }

        public void OnReturnToPool()
        {
            _triggered = false;
            _targetStats = null;

            transform.position = Vector2.zero;
            transform.localScale = Vector3.one;

            _anim.transform.localPosition = Vector3.zero;
        }

        private void Update()
        {
            if (!_targetStats || _triggered) 
                return;
        
            transform.position = Vector2.MoveTowards(transform.position, 
                _targetStats.transform.position, speed * Time.deltaTime);
        
            transform.right = transform.position - _targetStats.transform.position;

            if (Vector2.Distance(transform.position, _targetStats.transform.position) < Hit_Distance_Threshold)
            {
                _triggered = true;

                _anim.transform.localRotation = Quaternion.identity;
                _anim.transform.localPosition = animLocalPosition;

                transform.localRotation = Quaternion.identity;
                transform.localScale = hitScale;

                Invoke(nameof(DamageAndSelfDestroy), Damage_Delay);
                _anim.SetTrigger(AnimatorHashes.Hit);
            }
        }

        private void DamageAndSelfDestroy()
        {
            _targetStats.ApplyShock(true);

            _targetStats.TakeDamage(_damage);
            PoolManager.Instance.Return("shockStrike", gameObject, Return_Delay);
        }
    }
}
