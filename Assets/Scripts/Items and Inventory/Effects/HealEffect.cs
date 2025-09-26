using Managers;
using Stats;
using UnityEngine;

namespace Items_and_Inventory.Effects
{
    [CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item Effect/Heal", order = 0)]
    public class HealEffect : ItemEffect
    {
        [Range(0f, 1f)]
        [SerializeField] private float healPercent;

        public override void ExecuteEffect()
        {
            PlayerStats playerStats = PlayerManager.Instance.PlayerGameObject.GetComponent<PlayerStats>();

            int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);
    
            playerStats.IncreaseHealthBy(healAmount);
        }
    }
}
