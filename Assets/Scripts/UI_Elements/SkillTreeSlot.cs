using Core.Save_and_Load;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI_Elements
{
    public class SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
    {
        private static readonly Color UnlockedSkillColor = Color.white;
        
        [SerializeField] private int skillCost;
        [SerializeField] private string skillName;
        [TextArea]
        [SerializeField] private string skillDescription;
        [SerializeField] private Color lockedSkillColor;

        [SerializeField] private SkillTreeSlot[] shouldBeUnlocked;
        [SerializeField] private SkillTreeSlot[] shouldBeLocked;
    
        public bool Unlocked { get; private set; }
        
        private Image _skillImage;
        private Button _button;
        private UI _ui;

        private void OnValidate()
        {
            gameObject.name = "SkillTreeSlot_UI - " + skillName;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _ui = GetComponentInParent<UI>();
            _skillImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(UnlockSkillSlot);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(UnlockSkillSlot);
        }

        private void Start()
        {
            _skillImage.color = lockedSkillColor;

            if(Unlocked)
                _skillImage.color = UnlockedSkillColor;
        }

        private void UnlockSkillSlot() // need to refactor for all skills
        {
            if(PlayerManager.Instance.HasEnoughMoney(skillCost) == false)
                return;

            for (int i = 0; i < shouldBeUnlocked.Length; i++)
            {
                if(shouldBeUnlocked[i].Unlocked == false)
                {
                    Debug.Log("Cannot unlock skill");
                    return;
                }
            }

            for (int i = 0; i < shouldBeLocked.Length; i++)
            {
                if(shouldBeLocked[i].Unlocked == true)
                {
                    Debug.Log("Cannot unlock skill");
                    return;
                }
            }

            Unlocked = true;
            _skillImage.color = UnlockedSkillColor;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _ui.GetSkillTooltip().ShowTootTip(skillName, skillDescription, skillCost);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _ui.GetSkillTooltip().HideToolTip();
        }

        public void LoadData(GameData data)
        {
            if(data.GetSkillTree().TryGetValue(skillName, out bool value))
                Unlocked = value;
        }

        public void SaveData(ref GameData data)
        {
            if(data.GetSkillTree().TryGetValue(skillName, out _))
                data.GetSkillTree().Remove(skillName);
            
            data.GetSkillTree().Add(skillName, Unlocked);
        }
    }
}
