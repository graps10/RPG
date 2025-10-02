using Core;
using Enemies.Base;
using Items_and_Inventory;
using Skills;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class CrystalSkillController : MonoBehaviour
    {
        private const float Closest_Target_Min_Distance = 1f;
        
        private static readonly Vector2 growTargetScale = new(3f, 3f);
        
        [SerializeField] private float growSpeed = 5;
        [SerializeField] private LayerMask whatIsEnemy;
        
        private Player.Player _player;
        private Animator _anim => GetComponent<Animator>();
        private CircleCollider2D _cd => GetComponent<CircleCollider2D>();
        
        private float _moveSpeed;
        private float _crystalExistTimer;

        private bool _canExplode;
        private bool _canMove;
        private bool _canGrow;
        
        private Transform _closestTarget;

        public void SetupCrystal(float crystalDuration, bool canExplode, bool canMove, float moveSpeed, 
            Transform closestTarget, Player.Player player)
        {
            _player = player;
            _crystalExistTimer = crystalDuration;
            _canExplode = canExplode;
            _canMove = canMove;
            _moveSpeed = moveSpeed;
            _closestTarget = closestTarget;
        }

        public void ChooseRandomEnemy()
        {
            float radius = SkillManager.Instance.BlackHole.GetBlackHoleRadius();

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
            if (colliders.Length > 0)
                _closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
        }

        private void Update()
        {
            _crystalExistTimer -= Time.deltaTime;

            if (_crystalExistTimer < 0)
            {
                FinishCrystal();
            }

            if (_canMove)
                FaceClosestTarget();

            if (_canGrow)
                transform.localScale = Vector2.Lerp(transform.localScale, 
                    growTargetScale, growSpeed * Time.deltaTime);
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
                SelfDestroy();
        }

        public void SelfDestroy() => Destroy(gameObject);

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

                if (Vector2.Distance(transform.position, _closestTarget.position) < Closest_Target_Min_Distance)
                    FinishCrystal();
            }
        }
    }
}
