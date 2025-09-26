using TMPro;
using UnityEngine;

namespace UI_Elements
{
    public class ToolTip : MonoBehaviour
    {
        private const int Max_Text_Length = 12;
        private const float Font_Reduction_Factor = 0.8f;
        
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;

        private Vector2 _screenCenter;
    
        private void Start()
        {
            _screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        }

        protected virtual void AdjustPosition()
        {
            Vector2 mousePosition = Input.mousePosition;

            float newXOffset = mousePosition.x > _screenCenter.x ? -xOffset : xOffset;
            float newYOffset = mousePosition.y > _screenCenter.y ? -yOffset : yOffset;

            transform.position = new Vector2(mousePosition.x + newXOffset, mousePosition.y + newYOffset);
        }

        protected void AdjustFontSize(TextMeshProUGUI text)
        {
            if(text.text.Length > Max_Text_Length)
                text.fontSize *= Font_Reduction_Factor;
        }
    }
}
