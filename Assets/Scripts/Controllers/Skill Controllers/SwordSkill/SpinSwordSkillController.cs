using Core;
using Enemies.Base;
using UnityEngine;

namespace Controllers.Skill_Controllers.SwordSkill
{
    public class SpinSwordSkillController: SwordSkillController
    {
        private const float Spin_Move_Speed = 2.4f;
        private const float Spin_Hit_Radius = 1f;
        
        private static readonly Vector2 spinDirectionRange = new(-1f, 1f);
        
        private float _maxTravelDistance;
        private float _spinDuration;
        private float _spinDirection;
        
        private float _spinTimer;
        private bool _wasStopped;
        
        private float _hitTimer;
        private float _hitCooldown;
        
        private bool _isSpinning;

        public override void SetupSword(Vector2 dir, float gravityScale, 
            Player.Player player, float freezeTimeDuration, float returnSpeed)
        {
            base.SetupSword(dir, gravityScale, player, freezeTimeDuration, returnSpeed);
            
            _spinDirection = Mathf.Clamp(rb.velocity.x, spinDirectionRange.x, spinDirectionRange.y);
            anim.SetBool(AnimatorHashes.Rotation, true);
        }

        public void SetupSpin(bool isSpinning, float maxTravelDistance, float spinDuration, float hitCooldown)
        {
            _isSpinning = isSpinning;
            _maxTravelDistance = maxTravelDistance;
            _spinDuration = spinDuration;
            _hitCooldown = hitCooldown;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            SpinLogic();
        }

        protected override void StuckInto(Collider2D collision)
        {
            if (_isSpinning)
            {
                StopWhenSpinning();
                return;
            }
            
            base.StuckInto(collision);
        }

        private void StopWhenSpinning()
        {
            _wasStopped = true;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            _spinTimer = _spinDuration;
        }
        
        private void SpinLogic()
        {
            if (!_isSpinning) return;
            
            if (Vector2.Distance(player.transform.position, transform.position) > _maxTravelDistance && !_wasStopped)
                StopWhenSpinning();

            if (!_wasStopped) return;
            _spinTimer -= Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, 
                new Vector2(transform.position.x + _spinDirection,
                    transform.position.y), Spin_Move_Speed * Time.deltaTime);

            if (_spinTimer < 0)
            {
                _isSpinning = false;
                isReturning = true;
            }

            _hitTimer -= Time.deltaTime;

            if (_hitTimer < 0)
            {
                _hitTimer = _hitCooldown;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Spin_Hit_Radius);
                foreach (var hit in colliders)
                    if (hit.GetComponent<Enemy>() != null)
                        SwordSkillDamage(hit.GetComponent<Enemy>());
            }
        }
    }
}