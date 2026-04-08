using UnityEngine;

namespace Core.ObjectPool.Configs
{
    [CreateAssetMenu(fileName = "DeathBringerSpellConfig", menuName = "ObjectPool/Configs/Death Bringer Spell")]
    public class DeathBringerSpellPoolConfig : BasePoolConfig
    {
        [Header("Spell Cast Specific Settings")]
        [SerializeField] private Vector2 spellCastOffset;
        [SerializeField] private float spellStateCooldown;
        [SerializeField] private int amountOfSpells;
        [SerializeField] private float spellCooldown;
        [SerializeField] private LayerMask whatIsPlayer;
        
        public Vector2 SpellCastOffset => spellCastOffset;
        public float SpellStateCooldown => spellStateCooldown;
        public int AmountOfSpells => amountOfSpells;
        public float SpellCooldown => spellCooldown;
        public LayerMask WhatIsPlayer => whatIsPlayer;
    }
}