using TMPro;
using UnityEngine;

namespace UI_Elements
{
    public class SkillTooltip : ToolTip
    {
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillText;
        [SerializeField] private TextMeshProUGUI skillCost;
        [SerializeField] private float defaultNameFontSize;
    
        public void ShowTootTip(string skillName , string skillDescription, int price)
        {
            this.skillName.text = skillName;
            skillText.text = skillDescription;
            skillCost.text = "Cost: " + price;
        
            AdjustPosition();
            AdjustFontSize(this.skillName);

            gameObject.SetActive(true);
        }

        public void HideToolTip()
        {
            skillName.fontSize = defaultNameFontSize;
            gameObject.SetActive(false);
        }
    }
}
