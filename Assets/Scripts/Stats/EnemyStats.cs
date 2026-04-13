using Enemies.Base;
using Items_and_Inventory;
using Managers;
using UnityEngine;

namespace Stats
{
    public class EnemyStats : CharacterStats
    {
        [Header("Level Details")]
        [SerializeField] private int level = 1;

        [Range(0f, 1f)]
        [SerializeField] private float percentageModifier = 0.4f;
        [SerializeField] private int initialSoulsDropAmount = 100;
        
        private Enemy _enemy;
        private ItemDrop _myDropSystem;
        
        private Stat _soulsDropAmount = new();

        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponent<Enemy>();
            _myDropSystem = GetComponent<ItemDrop>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _soulsDropAmount.SetDefaultValue(initialSoulsDropAmount);
            ApplyLevelModifiers();
        }

        protected override void Die()
        {
            base.Die();

            if (_enemy != null)
            {
                _enemy.Die();
                _myDropSystem.GenerateDrop();
            }
        
            PlayerManager.Instance.AddCurrency(_soulsDropAmount.GetValue());
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

            Modify(_soulsDropAmount);
        }

        private void Modify(Stat stat)
        {
            for (int i = 1; i < level; i++)
            {
                float modifier = stat.GetValue() * percentageModifier;
                stat.AddModifier(Mathf.RoundToInt(modifier));
            }
        }
    }
}
