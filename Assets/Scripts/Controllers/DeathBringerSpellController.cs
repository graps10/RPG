using Managers;
using Stats;
using UnityEngine;

namespace Controllers
{
    public class DeathBringerSpellController : MonoBehaviour
    {
        [SerializeField] private Transform check;
        [SerializeField] private Vector2 boxSize;
        [SerializeField] private LayerMask whatIsPlayer;

        private CharacterStats _myStats;

        public void SetupSpell(CharacterStats stats) => _myStats = stats;

        private void AnimationTrigger()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatIsPlayer);

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<PlayerStats>() != null)
                {
                    hit.GetComponent<Entity.Entity>().SetupKnockbackDir(transform);
                    _myStats.DoDamage(hit.GetComponent<CharacterStats>());
                }
            }
        }

        private void SelfDestroy() => PoolManager.Instance.Return("deathBringerSpell", gameObject);

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(check.position, boxSize);
        }
    }
}
