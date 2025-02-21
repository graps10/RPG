using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if(_targetStats == null) return;
        
        if (TargetCanAvoidAttack(_targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);
        
        if(CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);
    }

    protected override void Die()
    {
        base.Die();

        player.Die();
        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DeacreaseHealthBy(int _damage)
    {
        base.DeacreaseHealthBy(_damage);

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if(currentArmor != null)
            currentArmor.Effect(player.transform);
    }

    public void AddStats(
        int _strength, int _agility, int _intelligence, int _vitality,
        int _damage, int _critChance, int _critPower,
        int _maxHealth, int _armor, int _evasion, int _magicResistance,
        int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        // Major Stats
        strength.AddModifier(_strength);
        agility.AddModifier(_agility);
        intelligence.AddModifier(_intelligence);
        vitality.AddModifier(_vitality);

        // Offensive Stats
        damage.AddModifier(_damage);
        critChance.AddModifier(_critChance);
        critPower.AddModifier(_critPower);

        // Defensive Stats
        maxHealth.AddModifier(_maxHealth);
        armor.AddModifier(_armor);
        evasion.AddModifier(_evasion);
        magicResistance.AddModifier(_magicResistance);

        // Magic Stats
        fireDamage.AddModifier(_fireDamage);
        iceDamage.AddModifier(_iceDamage);
        lightingDamage.AddModifier(_lightingDamage);
    }
    
    public void RemoveStats(
        int _strength, int _agility, int _intelligence, int _vitality,
        int _damage, int _critChance, int _critPower,
        int _maxHealth, int _armor, int _evasion, int _magicResistance,
        int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        // Major Stats
        strength.RemoveModifier(_strength);
        agility.RemoveModifier(_agility);
        intelligence.RemoveModifier(_intelligence);
        vitality.RemoveModifier(_vitality);

        // Offensive Stats
        damage.RemoveModifier(_damage);
        critChance.RemoveModifier(_critChance);
        critPower.RemoveModifier(_critPower);

        // Defensive Stats
        maxHealth.RemoveModifier(_maxHealth);
        armor.RemoveModifier(_armor);
        evasion.RemoveModifier(_evasion);
        magicResistance.RemoveModifier(_magicResistance);

        // Magic Stats
        fireDamage.RemoveModifier(_fireDamage);
        iceDamage.RemoveModifier(_iceDamage);
        lightingDamage.RemoveModifier(_lightingDamage);
    }
}
