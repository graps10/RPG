using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;
    
    void OnValidate() 
    {
        gameObject.name = "Stat - " + statName;

        if(statName != null)
            statNameText.text = statName;
    }

    void Start()
    {
        ui = GetComponentInParent<UI>();

        UpdateStatValueUI();
    }
    

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();

            SpecialUICalculations(playerStats);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statTooltip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.HideStatToolTip();
    }

    private void SpecialUICalculations(PlayerStats playerStats)
    {
        if (statType == StatType.health)
            statValueText.text = playerStats.GetMaxHealthValue().ToString();

        if (statType == StatType.damage)
            statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

        if (statType == StatType.critPower)
            statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();

        if (statType == StatType.critChance)
            statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();

        if (statType == StatType.evasion)
            statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();

        if (statType == StatType.magicResistance)
            statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue() * 3)).ToString();
    }
}
