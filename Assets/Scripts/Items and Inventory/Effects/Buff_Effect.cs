
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
    maxHealth,
    armor,
    evasion,
    magicResistance,

    // Magic Stats
    fireDamage,
    iceDamage,
    lightingDamage,
}

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item Effect/Buff effect", order = 0)]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExucuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(buffAmount, buffDuration, StatToModify());
    }

    private Stat StatToModify() 
    {
        switch (buffType)
        {
            // Major Stats
            case StatType.strength:
                return stats.strength;
            case StatType.agility:
                return stats.agility;
            case StatType.intelligence:
                return stats.intelligence;
            case StatType.vitality:
                return stats.vitality;

            // Offensive Stats
            case StatType.damage:
                return stats.damage;
            case StatType.critChance:
                return stats.critChance;
            case StatType.critPower:
                return stats.critPower;

            // Defensive Stats
            case StatType.maxHealth:
                return stats.maxHealth;
            case StatType.armor:
                return stats.armor;
            case StatType.evasion:
                return stats.evasion;
            case StatType.magicResistance:
                return stats.magicResistance;

            // Magic Stats
            case StatType.fireDamage:
                return stats.fireDamage;
            case StatType.iceDamage:
                return stats.iceDamage;
            case StatType.lightingDamage:
                return stats.lightingDamage;

            // Default case for safety
            default:
                throw new System.ArgumentOutOfRangeException($"Unhandled StatType: {buffType}");
        }
    }

}
