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
}
