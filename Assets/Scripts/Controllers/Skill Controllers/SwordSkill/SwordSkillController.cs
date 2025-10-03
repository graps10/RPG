using Core;
using Enemies.Base;
using Items_and_Inventory;
using Stats;
using UnityEngine;

namespace Controllers.Skill_Controllers.SwordSkill
{
    public class SwordSkillController: MonoBehaviour
    {
        private const float Catch_Distance_Threshold = 1f;     
        private const float Destroy_Sword_Delay = 7f;      
        
        protected Animator anim;     
        protected Rigidbody2D rb;     
        protected CircleCollider2D cd;     
        protected Player.Player player;      
        
        protected bool isReturning;      
        
        private bool _canRotate = true;     
        private float _returnSpeed;      
        private float _freezeTimeDuration;
        
        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            cd = GetComponent<CircleCollider2D>();
        }
        
        public virtual void SetupSword(Vector2 dir, float gravityScale,
            Player.Player player, float freezeTimeDuration, float returnSpeed)
        {
            this.player = player;
            _freezeTimeDuration = freezeTimeDuration;
            _returnSpeed = returnSpeed;

            rb.velocity = dir;
            rb.gravityScale = gravityScale;

            Invoke(nameof(DestroySword), Destroy_Sword_Delay);
        }
        
        protected virtual void FixedUpdate()
        {
            if (_canRotate)
                transform.right = rb.velocity;

            if (!isReturning) return;
            
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position,
                _returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < Catch_Distance_Threshold)
                player.CatchTheSword();
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (isReturning) return;

            if (collision.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                SwordSkillDamage(enemy);
            }

            player.Stats.DoDamage(collision.GetComponent<CharacterStats>());
            StuckInto(collision);
        }
        
        protected virtual void StuckInto(Collider2D collision)
        {
            _canRotate = false;
            cd.enabled = false;

            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            GetComponentInChildren<ParticleSystem>().Play();

            anim.SetBool(AnimatorHashes.Rotation, false);
            transform.parent = collision.transform;
        }
        
        protected void SwordSkillDamage(Enemy enemy)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            player.Stats.DoDamage(enemyStats);

            if (player.Skill.Sword.IsTimeStopUnlocked())
                enemy.FreezeTimeFor(_freezeTimeDuration);

            if (player.Skill.Sword.IsVulnerableUnlocked())
                enemyStats.MakeVulnerableFor(_freezeTimeDuration);

            ItemData_Equipment equippedAmulet = Inventory.Instance.GetEquippedItem(EquipmentType.Amulet);

            if (equippedAmulet != null)
                equippedAmulet.Effect(enemy.transform);
        }
        
        public void ReturnSword()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.parent = null;
            isReturning = true;
        }
        
        private void DestroySword() => Destroy(gameObject);
    }
}