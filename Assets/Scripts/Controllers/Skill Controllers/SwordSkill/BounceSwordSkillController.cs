using System.Collections.Generic;
using Core;
using Enemies.Base;
using UnityEngine;

namespace Controllers.Skill_Controllers.SwordSkill
{
    public class BounceSwordSkillController: SwordSkillController
    {
        private const float Bounce_Distance_Threshold = 0.1f;
        private const float Bounce_Target_Search_Radius = 10f;
        
        private float _bounceSpeed;
        private int _bounceAmount;
        
        private List<Transform> _enemyTarget;
        private int _targetIndex;
        
        private bool _isBouncing;
        
        public override void SetupSword(Vector2 dir, float gravityScale, 
            Player.Player player, float freezeTimeDuration, float returnSpeed)
        {
            base.SetupSword(dir, gravityScale, player, freezeTimeDuration, returnSpeed);
            anim.SetBool(AnimatorHashes.Rotation, true);
        }
        
        public void SetupBounce(bool isBouncing, int amountOfBounces, float bounceSpeed)
        {
            _isBouncing = isBouncing;
            _bounceAmount = amountOfBounces;
            _bounceSpeed = bounceSpeed;
            _enemyTarget = new List<Transform>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            BounceLogic();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            SetupTargetsForBounce(collision);
        }

        protected override void StuckInto(Collider2D collision)
        {
            if (_isBouncing && _enemyTarget.Count > 0) 
                return;
            
            base.StuckInto(collision);
        }

        private void BounceLogic()
        {
            if (!_isBouncing || _enemyTarget.Count <= 0) 
                return;
            
            if (_enemyTarget[_targetIndex] == null)
            {
                _enemyTarget.RemoveAt(_targetIndex);
                
                if (_enemyTarget.Count == 0)
                {
                    _isBouncing = false;
                    isReturning = true;
                    transform.right = transform.position - player.transform.position; 
                    return;
                }
                
                if (_targetIndex >= _enemyTarget.Count)
                    _targetIndex = 0;
                
                return; 
            }
            
            Vector2 targetDir = (_enemyTarget[_targetIndex].position - transform.position).normalized;
            transform.right = targetDir;
            
            transform.position = Vector2.MoveTowards(transform.position, 
                _enemyTarget[_targetIndex].position, _bounceSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, _enemyTarget[_targetIndex].position) >= Bounce_Distance_Threshold) 
                return;
            
            Enemy targetEnemy = _enemyTarget[_targetIndex].GetComponent<Enemy>();
            if (targetEnemy != null)
                SwordSkillDamage(targetEnemy);
            
            _targetIndex++;
            _bounceAmount--;

            if (_bounceAmount <= 0)
            {
                _isBouncing = false;
                isReturning = true;
                transform.right = transform.position - player.transform.position;
            }

            if (_targetIndex >= _enemyTarget.Count)
                _targetIndex = 0;
        }
        
        private void SetupTargetsForBounce(Collider2D collision)
        {
            if (!collision.TryGetComponent(out Enemy _)) 
                return;

            if (!_isBouncing || _enemyTarget.Count > 0) 
                return;
    
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Bounce_Target_Search_Radius);
    
            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent(out Enemy _))
                    _enemyTarget.Add(hit.transform);
            }
        }
        
        public override void ReturnToPool()
        {
            _isBouncing = false;
            _bounceAmount = 0;
            _targetIndex = 0;
    
            if (_enemyTarget != null)
                _enemyTarget.Clear();
        
            base.ReturnToPool();
        }
    }
}