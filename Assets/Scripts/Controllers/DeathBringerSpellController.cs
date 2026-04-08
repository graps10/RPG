using Core.ObjectPool;
using Core.ObjectPool.Configs;
using Stats;
using UnityEngine;

namespace Controllers
{
    public class DeathBringerSpellController : PooledObject
    {
        [SerializeField] private Transform check;
        [SerializeField] private Vector2 boxSize;
        

        private CharacterStats _myStats;
        private DeathBringerSpellPoolConfig _config;

        public void SetupSpell(CharacterStats stats, DeathBringerSpellPoolConfig config)
        {
            _myStats = stats;
            _config = config;
        }

        private void AnimationTrigger()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, _config.WhatIsPlayer);

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<PlayerStats>() != null)
                {
                    hit.GetComponent<Entity.Entity>().SetupKnockbackDir(transform);
                    _myStats.DoDamage(hit.GetComponent<CharacterStats>());
                }
            }
        }

        private void SelfDestroy() => ReturnToPool();

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(check.position, boxSize);
        }
    }
}
