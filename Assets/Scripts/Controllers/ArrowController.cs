using Core.ObjectPool;
using Core.ObjectPool.Configs;
using Core.Utilities;
using Stats;
using UnityEngine;

namespace Controllers
{
    public class ArrowController : PooledObject
    {
        private const int Arrow_Flip_Multiplier = -1;
        
        private Rigidbody2D _rb;
        private CapsuleCollider2D _cd;
        private ParticleSystem _trailFx;
    
        private CharacterStats _myStats;
        private ArrowPoolConfig _config;

        private LayerMask _targetLayer;
    
        private float _xVelocity;
    
        private bool _canMove;
        private bool _flipped;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _cd = GetComponent<CapsuleCollider2D>();
            _trailFx = GetComponentInChildren<ParticleSystem>();
        }
        
        private void OnEnable() 
        {
            _rb.isKinematic = false;
            _rb.constraints = RigidbodyConstraints2D.None;
            
            _cd.enabled = false;
            _canMove = false;
            _flipped = false;
    
            _trailFx.Play();
            
            CancelInvoke(nameof(ReturnToPool));
        }

        private void Update()
        {
            if (_canMove)
                _rb.velocity = new Vector2(_xVelocity, _rb.velocity.y);
        }

        public override void ReturnToPool()
        {
            _trailFx.Stop();
            _targetLayer = _config.WhatIsPlayer;
            transform.parent = null;
            
            CancelInvoke();
            base.ReturnToPool();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & _targetLayer) != 0)
            {
                /*collision.GetComponent<CharacterStats>()?.TakeDamage(damage);*/
                _myStats.DoDamage(collision.GetComponent<CharacterStats>());
                StuckInto(collision);
            }
        
            if (((1 << collision.gameObject.layer) & _config.WhatIsGround) != 0)
                StuckInto(collision);
        }

        public void SetupArrow(CharacterStats myStats, ArrowPoolConfig config, int facingDir)
        {
            _myStats = myStats;
            _config = config;
    
            _xVelocity = _config.Speed * facingDir;
            _targetLayer = _config.WhatIsPlayer;  
            
            _cd.enabled = true;
            _canMove = true;
            
            Invoke(nameof(ReturnToPool), _config.MissReturnDelay);
        }

        public void FlipArrow()
        {
            if (_flipped)
                return;

            _xVelocity *= Arrow_Flip_Multiplier;
            _flipped = true;

            transform.Rotate(TransformUtils.FlipAngle);
            _targetLayer = _config.WhatIsEnemy;
        }

        private void StuckInto(Collider2D collision)
        {
            _cd.enabled = false;
            _canMove = false;
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            
            transform.parent = collision.transform;
            
            CancelInvoke(nameof(ReturnToPool));
            Invoke(nameof(ReturnToPool), _config.HitReturnDelay);
        }
    }
}
