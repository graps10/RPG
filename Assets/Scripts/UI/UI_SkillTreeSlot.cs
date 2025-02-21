using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;
    
    private Image skillImage;

    void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    void Start()
    {
        ui = GetComponentInParent<UI>();

        skillImage = GetComponent<Image>();

        skillImage.color = lockedSkillColor;
    }

    public void UnlockSkillSlot()
    {
        if(PlayerManager.instance.HasEnoughMoney(skillCost) == false)
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if(shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if(shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        unlocked = true;
        skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowTootTip(skillName, skillDescription, skillCost);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideToolTip();
    }
}
