using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    private float xLimit;
    private float yLimit;
    
    void Start()
    {
        xLimit = Screen.width / 2f;
        yLimit = Screen.height / 2f;
    }

    public virtual void AdjustPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        float newXOffset = mousePosition.x > xLimit ? -xOffset : xOffset;
        float newYOffset = mousePosition.y > yLimit ? -yOffset : yOffset;

        transform.position = new Vector2(mousePosition.x + newXOffset, mousePosition.y + newYOffset);
    }

    public void AdjustFontSize(TextMeshProUGUI _text)
    {
        if(_text.text.Length > 12)
            _text.fontSize = _text.fontSize * 0.8f;
    }
}
