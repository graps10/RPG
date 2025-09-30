using Managers;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI_Elements
{
    public class StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const int Intelligence_Magic_Resist_Multiplier = 3;
        
        [SerializeField] private string statName;
        [SerializeField] private StatType statType;
        [SerializeField] private TextMeshProUGUI statValueText;
        [SerializeField] private TextMeshProUGUI statNameText;

        [TextArea]
        [SerializeField] private string statDescription;
        
        private UI _ui;
    
        private void OnValidate() 
        {
            gameObject.name = "Stat - " + statName;

            if(statName != null)
                statNameText.text = statName;
        }

        private void Start()
        {
            _ui = GetComponentInParent<UI>();

            UpdateStatValueUI();
        }
        
        public void UpdateStatValueUI()
        {
            PlayerStats playerStats = PlayerManager.Instance.PlayerGameObject.PlayerStats;

            if(playerStats != null)
            {
                statValueText.text = playerStats.GetStat(statType).GetValue().ToString();
                SpecialUICalculations(playerStats);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _ui.GetStatTooltip().ShowStatToolTip(statDescription);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _ui.GetStatTooltip().HideStatToolTip();
        }

        private void SpecialUICalculations(PlayerStats playerStats)
        {
            statValueText.text = statType switch
            {
                StatType.health => playerStats.GetMaxHealthValue().ToString(),
                StatType.damage => (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString(),
                StatType.critPower => (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString(),
                StatType.critChance => (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString(),
                StatType.evasion => (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString(),
                StatType.magicResistance => (playerStats.magicResistance.GetValue() +
                                             (playerStats.intelligence.GetValue() * Intelligence_Magic_Resist_Multiplier)).ToString(),
                _ => statValueText.text
            };
        }
    }
}
