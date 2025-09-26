using Controllers;
using Core;
using Core.Interfaces;
using Enemies.Base;
using Managers;
using UnityEngine;

namespace Enemies.DeathBringer
{
    public class EnemyDeathBringer : Enemy, IAttackable, ITeleportable, ISpellCaster
    {
        [Header("Teleport Specific")]
        [SerializeField] private BoxCollider2D teleportArea;
        [SerializeField] private Vector2 surroundingCheckSize;
        [SerializeField] private float chanceToTeleport = 25;
        [SerializeField] private float defaultChanceToTeleport = 25;
        [SerializeField] private float teleportGroundCheckDistance = 100f;
        [SerializeField] private float teleportPositionPadding = 3f;

        [Header("Spell Cast Specific")]
        [SerializeField] private GameObject spellPrefab;
        [SerializeField] private Vector2 spellCastOffset;
        [SerializeField] private float spellStateCooldown;
        [SerializeField] private int amountOfSpells;
        [SerializeField] private float spellCooldown;
        [HideInInspector] public float lastTimeCast;

        #region States
        public EnemyState AttackState { get; private set; }
        public EnemyState TeleportState { get; private set; }
        public EnemyState SpellCastState { get; private set; }
        
        #endregion
        
        private bool _bossFightBegun;

        protected override void Awake()
        {
            base.Awake();
            
            IdleState = new DeathBringerIdleState(this, StateMachine, AnimatorHashes.EnemyIdleState);
            BattleState = new DeathBringerBattleState(this, StateMachine, AnimatorHashes.EnemyBattleState);
            AttackState = new DeathBringerAttackState(this, StateMachine, AnimatorHashes.EnemyAttackState);
            TeleportState = new DeathBringerTeleportState(this, StateMachine, AnimatorHashes.EnemyTeleportState);
            SpellCastState = new DeathBringerSpellCastState(this, StateMachine, AnimatorHashes.EnemySpellCastState);
            DeadState = new DeathBringerDeadState(this, StateMachine, AnimatorHashes.EnemyIdleState);
        }

        protected override void Start()
        {
            base.Start();

            StateMachine.Initialize(IdleState);
        }
        
        public bool CanAttack()
        {
            if (Time.time >= GetLastTimeAttacked() + GetAttackCooldown())
            { 
                SetAttackCooldown(Random.Range(GetAttackCooldownRange().x, GetAttackCooldownRange().y));
                SetLastTimeAttacked(Time.time);
                return true;
            }

            return false;
        }
        
        public void CastSpell()
        {
            Player.Player player = PlayerManager.Instance.PlayerGameObject;
            float xOffset = 0;

            if (player.Rb.velocity.x != 0)
                xOffset = player.FacingDir * spellCastOffset.x;

            Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + spellCastOffset.y);

            GameObject newSpell = PoolManager.Instance.Spawn("deathBringerSpell", spellPosition, Quaternion.identity);
            newSpell.GetComponent<DeathBringerSpellController>().SetupSpell(Stats);
        }

        public bool CanTeleport()
        {
            if (Random.Range(0, 100) <= chanceToTeleport)
            {
                chanceToTeleport = defaultChanceToTeleport;
                return true;
            }
        
            return false;
        }

        public bool CanCastSpell()
        {
            if (Time.time >= lastTimeCast + spellStateCooldown)
            {
                return true;
            }

            return false;
        }
        
        public override void Die()
        {
            base.Die();

            StateMachine.ChangeState(DeadState);
        }
        
        public void FindAvailableTeleportPosition()
        {
            float x = Random.Range(teleportArea.bounds.min.x + teleportPositionPadding, 
                teleportArea.bounds.max.x - teleportPositionPadding);
            float y = Random.Range(teleportArea.bounds.min.y + teleportPositionPadding, 
                teleportArea.bounds.max.y - teleportPositionPadding);

            transform.position = new Vector3(x, y);
            transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (Cd.size.y / 2));

            if (!GroundBelow() || SomethingIsAround())
                FindAvailableTeleportPosition();
        }
    
        public void IncreaseChangeToTeleport(int value) => chanceToTeleport += value;
        
        public int GetAmountOfSpells() => amountOfSpells;
        public float GetSpellCooldown() => spellCooldown;
        
        public void SetBossFightBegin(bool begun) => _bossFightBegun = begun; // bullshit code
        public bool IsBossFightBegun() => _bossFightBegun;

        private RaycastHit2D GroundBelow() 
            => Physics2D.Raycast(transform.position, Vector2.down, teleportGroundCheckDistance, whatIsGround);

        private bool SomethingIsAround() 
            => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
            Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
        }
    }
}
