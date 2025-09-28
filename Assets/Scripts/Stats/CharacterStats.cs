using System.Collections;
using Components.FX;
using Controllers.Skill_Controllers;
using Core.ObjectPool;
using Enemies.Base;
using Managers;
using UnityEngine;

namespace Stats
{
    public enum StatType
    {
        // Major Stats
        strength,
        agility,
        intelligence,
        vitality,

        // Offensive Stats
        damage,
        critChance,
        critPower,

        // Defensive Stats
        health,
        armor,
        evasion,
        magicResistance,

        // Magic Stats
        fireDamage,
        iceDamage,
        lightingDamage,
    }

    public class CharacterStats : MonoBehaviour
    {
        #region Constants

        private const float Ignite_Chance = 0.3f;
        private const float Ignite_Damage_Cooldown = 0.3f;
        private const float Chill_Chance = 0.5f;
        private const float Shock_Chance = 0.5f;

        private const float Ignite_Damage_Multiplier = 0.2f;
        private const float Shock_Damage_Multiplier = 0.1f;

        private const float Chill_Slow_Percentage = 0.2f;
        private const float Vulnerable_Damage_Multiplier = 1.1f;

        private const int Vitality_Health_Multiplier = 5;
        private const int Shock_Evasion_Penalty = 20;
        private const float Armor_Reduction_Multiplier_When_Chilled = 0.8f;
        private const int Intelligence_Resistance_Multiplier = 3;
        
        private const float Crit_Power_Multiplier = 0.01f;
        private const float Min_Shock_Strike_Distance = 1f;
        private const float Shock_Strike_Radius = 25f;

        #endregion
        
        [Header("Major Stats")]
        [SerializeField] public Stat strength;
        [SerializeField] public Stat agility;
        [SerializeField] public Stat intelligence;
        [SerializeField] public Stat vitality;

        [Header("Offensive Stats")]
        [SerializeField] public Stat damage;
        [SerializeField] public Stat critChance;
        [SerializeField] public Stat critPower;

        [Header("Defensive Stats")]
        [SerializeField] public Stat maxHealth;
        [SerializeField] public Stat armor;
        [SerializeField] public Stat evasion;
        [SerializeField] public Stat magicResistance;

        [Header("Magic Stats")]
        [SerializeField] public Stat fireDamage;
        [SerializeField] public Stat iceDamage;
        [SerializeField] public Stat lightingDamage;

        [SerializeField] private float ailmentsDuration = 4;
        [SerializeField] private GameObject shockStrikePrefab;
        
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }
        
        public System.Action OnHealthChanged;
        
        private bool _isIgnited;
        private bool _isChilled;
        private bool _isShocked;
        
        private bool _isInvincible;
        private bool _isVulnerable;
        
        private float _ignitedTimer;
        private float _chilledTimer;
        private float _shockedTimer;
        
        private float _igniteDamageTimer;
        private int _igniteDamage;
        
        private int _shockDamage;
        
        private EntityFX _fx;

        protected virtual void Start()
        {
            CurrentHealth = GetMaxHealthValue();

            _fx = GetComponent<EntityFX>();
        }

        protected virtual void Update()
        {
            _ignitedTimer -= Time.deltaTime;
            _chilledTimer -= Time.deltaTime;
            _shockedTimer -= Time.deltaTime;

            _igniteDamageTimer -= Time.deltaTime;

            if (_ignitedTimer < 0) _isIgnited = false;
            if (_chilledTimer < 0) _isChilled = false;
            if (_shockedTimer < 0) _isShocked = false;
            
            if (_isIgnited) ApplyIgniteDamage();
        }

        public void MakeInvincible(bool invincible) => _isInvincible = invincible;
        public void MakeVulnerableFor(float duration) 
            => StartCoroutine(VulnerableCoroutine(duration));

        private IEnumerator VulnerableCoroutine(float duration)
        {
            _isVulnerable = true;
            yield return new WaitForSeconds(duration);
            _isVulnerable = false;
        }

        public virtual void IncreaseStatBy(int modifier, float duration, Stat statToModify)
        {
            StartCoroutine(StatModCoroutine(modifier, duration, statToModify));
        }

        private static IEnumerator StatModCoroutine(int modifier, float duration, Stat statToModify)
        {
            statToModify.AddModifier(modifier);
            yield return new WaitForSeconds(duration);
            statToModify.RemoveModifier(modifier);
        }

        public virtual void DoDamage(CharacterStats targetStats)
        {
            if (targetStats == null) return;
            if (targetStats._isInvincible) return;

            bool criticalStrike = false;

            if (TargetCanAvoidAttack(targetStats)) return;

            targetStats.GetComponent<Entity.Entity>().SetupKnockbackDir(transform);

            int totalDamage = damage.GetValue() + strength.GetValue();

            if (CanCrit())
            {
                totalDamage = CalculateCriticalDamage(totalDamage);
                criticalStrike = true;
            }

            _fx.CreateHitFX(targetStats.transform, criticalStrike);

            totalDamage = CheckTargetArmor(targetStats, totalDamage);

            targetStats.TakeDamage(totalDamage);

            DoMagicalDamage(targetStats);
        }

        #region Magical Damage and ailments

        public virtual void DoMagicalDamage(CharacterStats targetStats)
        {
            int _fireDamage = fireDamage.GetValue();
            int _iceDamage = iceDamage.GetValue();
            int _lightingDamage = lightingDamage.GetValue();

            int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

            totalMagicalDamage = CheckTargetResistance(targetStats, totalMagicalDamage);
            targetStats.TakeDamage(totalMagicalDamage);

            if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0) return;

            AttemptToApplyAilments(targetStats, _fireDamage, _iceDamage, _lightingDamage);
        }

        private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
        {
            bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
            bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
            bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

            while (!canApplyIgnite && !canApplyChill && !canApplyShock)
            {
                if (Random.value < Ignite_Chance && _fireDamage > 0)
                {
                    canApplyIgnite = true;
                    _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                    return;
                }

                if (Random.value < Chill_Chance && _iceDamage > 0)
                {
                    canApplyChill = true;
                    _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                    return;
                }

                if (Random.value < Shock_Chance && _lightingDamage > 0)
                {
                    canApplyShock = true;
                    _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                    return;
                }
            }

            if (canApplyIgnite)
                _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * Ignite_Damage_Multiplier));

            if (canApplyShock)
                _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * Shock_Damage_Multiplier));
            
            _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
        }

        private void ApplyAilments(bool _ignite, bool _chill, bool _shock)
        {
            bool canApplyIgnite = !_isIgnited && !_isChilled && !_isShocked;
            bool canApplyChill = !_isIgnited && !_isChilled && !_isShocked;
            bool canApplyShock = !_isIgnited && !_isChilled;

            if (_ignite && canApplyIgnite)
            {
                _isIgnited = _ignite;
                _ignitedTimer = ailmentsDuration;

                _fx?.IgniteFxFor(ailmentsDuration);
            }

            if (_chill && canApplyChill)
            {
                _isChilled = _chill;
                _chilledTimer = ailmentsDuration;
                
                GetComponent<Entity.Entity>().SlowEntityBy(Chill_Slow_Percentage, ailmentsDuration);

                _fx?.ChillFxFor(ailmentsDuration);
            }

            if (_shock && canApplyShock)
            {
                if (!_isShocked)
                    ApplyShock(_shock);
                else
                {
                    if (GetComponent<Player.Player>() != null) 
                        return;
                    
                    HitNearestTargetWithShockStrike();
                }
            }
        }

        private void ApplyIgniteDamage()
        {
            if (_igniteDamageTimer < 0)
            {
                DecreaseHealthBy(_igniteDamage);

                _igniteDamageTimer = Ignite_Damage_Cooldown;
            }
        }

        public void ApplyShock(bool shock)
        {
            if (_isShocked) return;

            _isShocked = shock;
            _shockedTimer = ailmentsDuration;

            if (_fx != null)
                _fx.ShockFxFor(ailmentsDuration);
        }

        public void SetupIgniteDamage(int damage) => _igniteDamage = damage;

        public void SetupShockStrikeDamage(int damage) => _shockDamage = damage;

        #endregion

        public virtual void TakeDamage(int damage)
        {
            if (_isInvincible) return;

            DecreaseHealthBy(damage);
        
            if (_fx != null && damage > 0)
            {
                _fx.CreatePopUpText(damage.ToString());
                GetComponent<Entity.Entity>().DamageImpact();
                _fx.StartCoroutine("FlashFX");
            }
        }

        public void IncreaseHealthBy(int amount)
        {
            CurrentHealth += amount;
            OnHealthChanged?.Invoke();

            if (CurrentHealth > GetMaxHealthValue())
                CurrentHealth = GetMaxHealthValue();
        }

        protected virtual void OnEvasion() { }

        protected virtual void DecreaseHealthBy(int _damage)
        {
            if (_isVulnerable)
                _damage = Mathf.RoundToInt(_damage * Vulnerable_Damage_Multiplier);

            if (IsDead) return;

            CurrentHealth -= _damage;
            OnHealthChanged?.Invoke();

            if (CurrentHealth <= 0)
                Die();
        }

        public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * Vitality_Health_Multiplier;

        protected virtual void Die()
        {
            IsDead = true;
        }

        public void KillEntity()
        {
            if (!IsDead)
                Die();
        }

        #region Stat calculations

        protected bool TargetCanAvoidAttack(CharacterStats targetStats)
        {
            int totalEvasion = targetStats.evasion.GetValue() + targetStats.agility.GetValue();

            if (_isShocked)
                totalEvasion += Shock_Evasion_Penalty;

            if (Random.Range(0, 100) < totalEvasion)
            {
                targetStats.OnEvasion();
                return true;
            }

            return false;
        }

        protected int CheckTargetArmor(CharacterStats targetStats, int totalDamage)
        {
            if (targetStats._isChilled)
                totalDamage -= Mathf.RoundToInt(targetStats.armor.GetValue() * Armor_Reduction_Multiplier_When_Chilled);
            else
                totalDamage -= targetStats.armor.GetValue();

            totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
            return totalDamage;
        }

        private int CheckTargetResistance(CharacterStats targetStats, int totalMagicalDamage)
        {
            totalMagicalDamage -= targetStats.magicResistance.GetValue() 
                                  + (targetStats.intelligence.GetValue() * Intelligence_Resistance_Multiplier);
            
            totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
            return totalMagicalDamage;
        }

        protected bool CanCrit()
        {
            int totalCriticalChance = critChance.GetValue() + agility.GetValue();

            if (Random.Range(0, 100) < totalCriticalChance)
                return true;

            return false;
        }

        protected int CalculateCriticalDamage(int damage)
        {
            float totalCritPower = (critPower.GetValue() + strength.GetValue()) * Crit_Power_Multiplier;
            float critDamage = damage * totalCritPower;

            return Mathf.RoundToInt(critDamage);
        }

        private void HitNearestTargetWithShockStrike()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Shock_Strike_Radius);

            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null &&
                    Vector2.Distance(transform.position, hit.transform.position) > Min_Shock_Strike_Distance)
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                    if (distanceToEnemy < closestDistance)
                    {
                        closestDistance = distanceToEnemy;
                        closestEnemy = hit.transform;
                    }
                }

                if (closestEnemy == null)
                    closestEnemy = transform;
            }

            if (closestEnemy != null)
            {
                GameObject newShockStrike = PoolManager.Instance.Spawn(PoolNames.SHOCK_STRIKE, transform.position, Quaternion.identity, shockStrikePrefab);

                if (newShockStrike)
                    newShockStrike.GetComponent<ShockStrikeController>().
                        Setup(_shockDamage, closestEnemy.GetComponent<CharacterStats>());
            }
        }

        #endregion

        public Stat GetStat(StatType _statType)
        {
            return _statType switch
            {
                // Major Stats
                StatType.strength => strength,
                StatType.agility => agility,
                StatType.intelligence => intelligence,
                StatType.vitality => vitality,
                // Offensive Stats
                StatType.damage => damage,
                StatType.critChance => critChance,
                StatType.critPower => critPower,
                // Defensive Stats
                StatType.health => maxHealth,
                StatType.armor => armor,
                StatType.evasion => evasion,
                StatType.magicResistance => magicResistance,
                // Magic Stats
                StatType.fireDamage => fireDamage,
                StatType.iceDamage => iceDamage,
                StatType.lightingDamage => lightingDamage,
                _ => throw new System.ArgumentOutOfRangeException($"Unhandled StatType: {_statType}")
            };
        }
    }
}