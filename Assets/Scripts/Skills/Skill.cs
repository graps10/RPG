using System;
using Enemies.Base;
using Managers;
using UI_Elements;
using UnityEngine;

namespace Skills
{
    public class Skill : MonoBehaviour
    {
        private const float Enemy_Search_Radius = 25f;
        
        [SerializeField] private float cooldownDuration;

        protected Player.Player player;
        protected float cooldownRemaining;

        protected virtual void Start()
        {
            player = PlayerManager.Instance.PlayerGameObject;

            CheckUnlock();
        }

        protected virtual void Update()
        {
            cooldownRemaining -= Time.deltaTime;
        }
        
        public float GetCooldownDuration() => cooldownDuration; 
        public float GetCooldownRemaining() => cooldownRemaining;

        protected virtual void CheckUnlock() { }

        protected static void TryUnlock(SkillTreeSlot button, ref bool skillFlag, Action completeCallback = null)
        {
            if (button == null || !button.Unlocked) 
                return;
            
            skillFlag = true;
            completeCallback?.Invoke();
        }

        public virtual bool CanUseSkill()
        {
            if (cooldownRemaining < 0)
            {
                UseSkill();
                cooldownRemaining = cooldownDuration;
                return true;
            }
        
            player.Fx.CreatePopUpText("Cooldown");
            return false;
        }
        
        public virtual void UseSkill() { }

        protected void SetCooldownDuration(float value) => cooldownDuration = value;
        
        protected virtual Transform FindClosestEnemy(Transform checkTransform)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(checkTransform.position, Enemy_Search_Radius);

            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() == null) continue;
            
                float distanceToEnemy = Vector2.Distance(checkTransform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            return closestEnemy;
        }
    }
}
