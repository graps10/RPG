using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;
    
    public void ShowTootTip(string _skillName , string _skillDescription, int _price)
    {
        skillName.text = _skillName;
        skillText.text = _skillDescription;
        skillCost.text = "Cost: " + _price;
        
        AdjustPosition();
        AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }
}
