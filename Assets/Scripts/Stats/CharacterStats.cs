using System.Collections;
using UnityEngine;

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
    private EntityFX fx;

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

    public bool IsIgnited { get; private set; }
    public bool IsChilled { get; private set; }
    public bool IsShocked { get; private set; }

    [SerializeField] private float ailmentsDuration = 4;

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = 0.3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;


    public int CurrentHealth { get; private set; }

    public System.Action onHealthChanged;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        CurrentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0) IsIgnited = false;

        if (chilledTimer < 0) IsChilled = false;

        if (shockedTimer < 0) IsShocked = false;


        if (IsIgnited) ApplyIgniteDamage();
    }

    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableCoroutine(_duration));

    private IEnumerator VulnerableCoroutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (_targetStats == null) return;

        if (_targetStats.isInvincible) return;

        bool criticalStrike = false;

        if (TargetCanAvoidAttack(_targetStats)) return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFX(_targetStats.transform, criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);
    }

    #region Magical Damage and ailments

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0) return;

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * 0.1f));


        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !IsIgnited && !IsChilled && !IsShocked;
        bool canApplyChill = !IsIgnited && !IsChilled && !IsShocked;
        bool canApplyShock = !IsIgnited && !IsChilled;

        if (_ignite && canApplyIgnite)
        {
            IsIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }

        if (_chill && canApplyChill)
        {
            IsChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercantage = 0.2f;
            GetComponent<Entity>().SlowEntityBy(slowPercantage, ailmentsDuration);

            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shock && canApplyShock)
        {
            if (!IsShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null) return;

                HitNearestTargetWithShockStrike();
            }
        }
    }

    void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DeacreaseHealthBy(igniteDamage);

            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (IsShocked) return;

        IsShocked = _shock;
        shockedTimer = ailmentsDuration;

        if (fx != null)
            fx.ShockFxFor(ailmentsDuration);
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion
    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible) return;

        DeacreaseHealthBy(_damage);

        if (fx != null)
        {
            GetComponent<Entity>().DamageImpact();
            fx.StartCoroutine("FlashFX");
        }

    }

    public void IncreaseHealthBy(int _amount)
    {
        CurrentHealth += _amount;
        onHealthChanged?.Invoke();

        if (CurrentHealth > GetMaxHealthValue())
            CurrentHealth = GetMaxHealthValue();
    }

    public virtual void OnEvasion()
    {

    }

    protected virtual void DeacreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        if (isDead) return;

        CurrentHealth -= _damage;
        onHealthChanged?.Invoke();

        if (_damage > 0)
            fx?.CreatePopUpText(_damage.ToString());

        if (CurrentHealth <= 0)
            Die();
    }

    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;

    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;

    #region Stat calculations

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (IsShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.IsChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;

        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null &&
            Vector2.Distance(transform.position, hit.transform.position) > 1)
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
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().
                Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    #endregion

    public Stat GetStat(StatType _statType)
    {
        switch (_statType)
        {
            // Major Stats
            case StatType.strength:
                return strength;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.vitality:
                return vitality;

            // Offensive Stats
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critPower:
                return critPower;

            // Defensive Stats
            case StatType.health:
                return maxHealth;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.magicResistance:
                return magicResistance;

            // Magic Stats
            case StatType.fireDamage:
                return fireDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.lightingDamage:
                return lightingDamage;

            // Default case for safety
            default:
                throw new System.ArgumentOutOfRangeException($"Unhandled StatType: {_statType}");
        }
    }
}
