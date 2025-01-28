using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;


    [Header("Level Details")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier = 0.4f;

    protected override void Start()
    {
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        enemy.Die();

        myDropSystem.GenerateDrop();
    }

    private void ApplyLevelModifiers()
    {
        // Major Stats
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        // Offensive Stats
        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        // Defensive Stats
        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        // Magic Stats
        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);
    }

    private void Modify(Stat _stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = _stat.GetValue() * percantageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }
}
