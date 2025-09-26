using Core.ObjectPool;
using Core.Utilities;
using Managers;
using Stats;
using UnityEngine;

namespace Controllers
{
    public class ArrowController : MonoBehaviour, ISpawnedPooledObject
    {
        private const float Arrow_Return_To_Pool_Delay = 6f;
        private const int Arrow_Flip_Multiplier = -1;
    
        [SerializeField] private int damage;
        [SerializeField] private LayerMask whatIsPlayer;
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private LayerMask whatIsGround;
    
        private Rigidbody2D _rb;
        private CapsuleCollider2D _cd;
        private ParticleSystem _trailFx;
    
        private CharacterStats _myStats;

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

        private void Update()
        {
            if (_canMove)
                _rb.velocity = new Vector2(_xVelocity, _rb.velocity.y);
        }

        public void OnSpawn()
        {
            _rb.isKinematic = false;
            _rb.constraints = RigidbodyConstraints2D.None;
            _cd.enabled = true;

            _canMove = true;
            _flipped = false;
            
            _targetLayer = whatIsPlayer;
            _trailFx.Play();

            PoolManager.Instance.Return("arrow", gameObject, Arrow_Return_To_Pool_Delay);
        }

        public void OnReturnToPool()
        {
            _targetLayer = whatIsPlayer;
            _trailFx.Stop();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & _targetLayer) != 0)
            {
                /*collision.GetComponent<CharacterStats>()?.TakeDamage(damage);*/
                _myStats.DoDamage(collision.GetComponent<CharacterStats>());
                StuckInto(collision);
            }
        
            if (((1 << collision.gameObject.layer) & whatIsGround) != 0)
                StuckInto(collision);
        }

        public void SetupArrow(CharacterStats myStats, float speed)
        {
            _myStats = myStats;
            _xVelocity = speed;
        }

        public void FlipArrow()
        {
            if (_flipped)
                return;

            _xVelocity *= Arrow_Flip_Multiplier;
            _flipped = true;

            transform.Rotate(TransformUtils.FlipAngle);

            _targetLayer = whatIsEnemy;
        }

        private void StuckInto(Collider2D collision)
        {
            _trailFx.Stop();
            _cd.enabled = false;

            _canMove = false;
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.parent = collision.transform;
        }
    }
}
