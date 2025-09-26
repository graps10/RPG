using UI_Elements;
using UnityEngine;
using UnityEngine.UI;

namespace Skills.Skills
{
    public class ParrySkill : Skill
    {
        [Header("Parry")]
        [SerializeField] private SkillTreeSlot parryUnlockButton;
        
        [Header("Parry restore")]
        [SerializeField] private SkillTreeSlot restoreUnlockButton;
        [Range(0f, 1f)]
        [SerializeField] private float restoreHealthPercentage;
        
        [Header("Parry with mirage")]
        [SerializeField] private SkillTreeSlot parryWithMirageUnlockButton;
        
        private bool _parryUnlocked;
        private bool _restoreUnlocked;
        private bool _parryWithMirageUnlocked;

        private void OnEnable()
        {
            parryUnlockButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(parryUnlockButton, ref _parryUnlocked));
            
            restoreUnlockButton.GetComponent<Button>().onClick.AddListener(
                () =>  TryUnlock(restoreUnlockButton, ref _restoreUnlocked));
            
            parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(parryWithMirageUnlockButton, ref _parryWithMirageUnlocked));
        }

        private void OnDisable()
        {
            if(parryUnlockButton != null)
                parryUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(restoreUnlockButton != null)
                restoreUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(parryWithMirageUnlockButton != null)
                parryWithMirageUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        protected override void CheckUnlock()
        {
            TryUnlock(parryUnlockButton, ref _parryUnlocked);
            TryUnlock(restoreUnlockButton, ref _restoreUnlocked);
            TryUnlock(parryWithMirageUnlockButton, ref _parryWithMirageUnlocked);
        }
        
        public bool IsParryUnlocked() => _parryUnlocked;

        public override void UseSkill()
        {
            base.UseSkill();

            if (_restoreUnlocked)
            {
                int restoreAmount = Mathf.RoundToInt(player.Stats.GetMaxHealthValue() * restoreHealthPercentage);
                player.Stats.IncreaseHealthBy(restoreAmount);
            }
        }

        public void MakeMirageOnParry(Transform _respawnTransform)
        {
            if (_parryWithMirageUnlocked)
                SkillManager.Instance.Clone.CreateCloneWithDelay(_respawnTransform);
        }
    }
}
