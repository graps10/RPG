using Controllers;
using Core;
using Core.Interfaces;
using Core.ObjectPool.Configs;
using Core.ObjectPool.Configs.Controllers;
using Enemies.Base;
using Managers;
using UnityEngine;
using PoolManager = Core.ObjectPool.PoolManager;

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
        [SerializeField] private DeathBringerSpellPoolConfig spellConfig;
        [HideInInspector] public float lastTimeCast;

        #region States
        public EnemyState AttackState { get; private set; }
        public EnemyState TeleportState { get; private set; }
        public EnemyState SpellCastState { get; private set; }
        
        #endregion

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
                xOffset = player.FacingDir * spellConfig.SpellCastOffset.x;

            Vector3 spellPosition 
                = new Vector3(player.transform.position.x + xOffset, 
                    player.transform.position.y + spellConfig.SpellCastOffset.y);

            GameObject newSpell = PoolManager.Instance.Spawn(spellConfig.Prefab, spellPosition, Quaternion.identity);
            newSpell.GetComponent<DeathBringerSpellController>().SetupSpell(Stats, spellConfig);
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
            if (Time.time >= lastTimeCast + spellConfig.SpellStateCooldown)
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
        
        public void StartBattle()
        {
            if (StateMachine.CurrentState == IdleState) 
                StateMachine.ChangeState(BattleState);
        }
        
        public void FindAvailableTeleportPosition()
        {
            float x = Random.Range(teleportArea.bounds.min.x + teleportPositionPadding, 
                teleportArea.bounds.max.x - teleportPositionPadding);
            float y = Random.Range(teleportArea.bounds.min.y + teleportPositionPadding, 
                teleportArea.bounds.max.y - teleportPositionPadding);

            transform.position = new Vector3(x, y);
            transform.position = new Vector3(transform.position.x, 
                transform.position.y - GroundBelow().distance + (Cd.size.y / 2));

            if (!GroundBelow() || SomethingIsAround())
                FindAvailableTeleportPosition();
        }
    
        public void IncreaseTheChanceToTeleport(int value) => chanceToTeleport += value;
        
        public int GetAmountOfSpells() => spellConfig.AmountOfSpells;
        public float GetSpellCooldown() => spellConfig.SpellCooldown;

        private RaycastHit2D GroundBelow() 
            => Physics2D.Raycast(transform.position, 
                Vector2.down, teleportGroundCheckDistance, whatIsGround);

        private bool SomethingIsAround() 
            => Physics2D.BoxCast(transform.position, 
                surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, 
                transform.position.y - GroundBelow().distance));
            Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
        }
    }
}
