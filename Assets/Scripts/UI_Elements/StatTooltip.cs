using TMPro;
using UnityEngine;

namespace UI_Elements
{
    public class StatTooltip : ToolTip
    {
        [SerializeField] private TextMeshProUGUI description;

        public void ShowStatToolTip(string text)
        {
            description.text = text;
            AdjustPosition();
        
            gameObject.SetActive(true);
        }

        public void HideStatToolTip()
        {
            description.text = string.Empty;
            gameObject.SetActive(false);
        }
    }
}
