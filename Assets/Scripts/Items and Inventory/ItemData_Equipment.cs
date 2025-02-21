using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment", order = 0)]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;


    [Header("Major Stats")]
    [SerializeField] private int strength;
    [SerializeField] private int agility;
    [SerializeField] private int intelligence;
    [SerializeField] private int vitality;

    [Header("Offensive Stats")]
    [SerializeField] private int damage;
    [SerializeField] private int critChance;
    [SerializeField] private int critPower;

    [Header("Defensive Stats")]
    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private int evasion;
    [SerializeField] private int magicResistance;

    [Header("Magic Stats")]
    [SerializeField] private int fireDamage;
    [SerializeField] private int iceDamage;
    [SerializeField] private int lightingDamage;

    [Header("Craft Requirements")]
    [SerializeField] private List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        // Major Stats
        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");

        // Offensive Stats
        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Critical Chance");
        AddItemDescription(critPower, "Critical Power");

        // Defensive Stats
        AddItemDescription(health, "Health");
        AddItemDescription(armor, "Armor");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(magicResistance, "Magic Resistance");

        // Magic Stats
        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        AddItemDescription(lightingDamage, "Lightning Damage");

        for (int i = 0; i < itemEffects.Length; i++)
        {
            if(itemEffects[i].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Unique: " + itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }

        if(descriptionLength < 5)
        {
            for(int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }
        
        return sb.ToString();
    }

    public void Effect(Transform _enemyPosition)
    {
        foreach(var item in itemEffects)
        {
            item.ExucuteEffect(_enemyPosition);
        }
    }
    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
    
        playerStats.AddStats(
        strength, agility, intelligence, vitality,
        damage, critChance, critPower,
        health, armor, evasion, magicResistance,
        fireDamage, iceDamage, lightingDamage
    );
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.RemoveStats(
        strength, agility, intelligence, vitality,
        damage, critChance, critPower,
        health, armor, evasion, magicResistance,
        fireDamage, iceDamage, lightingDamage
    );
    }

    public List<InventoryItem> GetCraftingMaterials() => craftingMaterials;

    private void AddItemDescription(int _value, string _name)
    {
        if(_value != 0)
        {
            if(sb.Length > 0)
                sb.AppendLine();
            
            if(_value > 0)
                sb.Append("+ " + _value + " " + _name);

            descriptionLength ++;
        }
    }
}
