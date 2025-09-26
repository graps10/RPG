using Items_and_Inventory;
using Managers;
using UnityEngine;

namespace Stats
{
    public class PlayerStats : CharacterStats
    {
        private const float High_Damage_Threshold = 0.3f;
        private static Vector2 highDamageKnockback = new(10, 6);
        
        private Player.Player _player;

        protected override void Start()
        {
            base.Start();

            _player = GetComponent<Player.Player>();
        }

        protected override void OnEvasion()
        {
            _player.Skill.Dodge.CreateMirageOnDodge();
        }

        public void CloneDoDamage(CharacterStats targetStats, float multiplier)
        {
            if (targetStats == null) return;

            if (TargetCanAvoidAttack(targetStats)) return;

            int totalDamage = damage.GetValue() + strength.GetValue();

            if (multiplier > 0)
                totalDamage = Mathf.RoundToInt(totalDamage * multiplier);

            if (CanCrit())
            {
                totalDamage = CalculateCriticalDamage(totalDamage);
            }

            totalDamage = CheckTargetArmor(targetStats, totalDamage);

            targetStats.TakeDamage(totalDamage);

            DoMagicalDamage(targetStats);
        }

        protected override void Die()
        {
            base.Die();

            _player.Die();

            GameManager.Instance.SetLostCurrencyAmount(PlayerManager.Instance.GetCurrency());
            PlayerManager.Instance.SetCurrency(0);

            GetComponent<PlayerItemDrop>()?.GenerateDrop();
        }

        protected override void DecreaseHealthBy(int damage)
        {
            base.DecreaseHealthBy(damage);

            if (damage > GetMaxHealthValue() * High_Damage_Threshold)
            {
                _player.SetupKnockbackPower(highDamageKnockback);
                _player.Fx.ScreenShake(_player.Fx.GetShakeHighDamage());
                AudioManager.Instance.PlaySFX(32, null);
            }
        
            ItemData_Equipment currentArmor = Inventory.Instance.GetEquipment(EquipmentType.Armor);

            if (currentArmor != null)
                currentArmor.Effect(_player.transform);
        }

        public void AddStats(
            int strength, int agility, int intelligence, int vitality,
            int damage, int critChance, int critPower,
            int maxHealth, int armor, int evasion, int magicResistance,
            int fireDamage, int iceDamage, int lightingDamage)
        {
            // Major Stats
            this.strength.AddModifier(strength);
            this.agility.AddModifier(agility);
            this.intelligence.AddModifier(intelligence);
            this.vitality.AddModifier(vitality);

            // Offensive Stats
            this.damage.AddModifier(damage);
            this.critChance.AddModifier(critChance);
            this.critPower.AddModifier(critPower);

            // Defensive Stats
            this.maxHealth.AddModifier(maxHealth);
            this.armor.AddModifier(armor);
            this.evasion.AddModifier(evasion);
            this.magicResistance.AddModifier(magicResistance);

            // Magic Stats
            this.fireDamage.AddModifier(fireDamage);
            this.iceDamage.AddModifier(iceDamage);
            this.lightingDamage.AddModifier(lightingDamage);
        }

        public void RemoveStats(
            int strength, int agility, int intelligence, int vitality,
            int damage, int critChance, int critPower,
            int maxHealth, int armor, int evasion, int magicResistance,
            int fireDamage, int iceDamage, int lightingDamage)
        {
            // Major Stats
            this.strength.RemoveModifier(strength);
            this.agility.RemoveModifier(agility);
            this.intelligence.RemoveModifier(intelligence);
            this.vitality.RemoveModifier(vitality);

            // Offensive Stats
            this.damage.RemoveModifier(damage);
            this.critChance.RemoveModifier(critChance);
            this.critPower.RemoveModifier(critPower);

            // Defensive Stats
            this.maxHealth.RemoveModifier(maxHealth);
            this.armor.RemoveModifier(armor);
            this.evasion.RemoveModifier(evasion);
            this.magicResistance.RemoveModifier(magicResistance);

            // Magic Stats
            this.fireDamage.RemoveModifier(fireDamage);
            this.iceDamage.RemoveModifier(iceDamage);
            this.lightingDamage.RemoveModifier(lightingDamage);
        }
    }
}
