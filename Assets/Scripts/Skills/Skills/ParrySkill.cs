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
            parryUnlockButton.OnUnlocked += UnlockParry;
            restoreUnlockButton.OnUnlocked += UnlockRestore;
            parryWithMirageUnlockButton.OnUnlocked += UnlockParryWithMirage;
        }

        private void OnDisable()
        {
            if(parryUnlockButton != null)
                parryUnlockButton.OnUnlocked -= UnlockParry;
            
            if(restoreUnlockButton != null)
                restoreUnlockButton.OnUnlocked -= UnlockRestore;
            
            if(parryWithMirageUnlockButton != null)
                parryWithMirageUnlockButton.OnUnlocked -= UnlockParryWithMirage;
        }

        protected override void CheckUnlock()
        {
            UnlockParry();
            UnlockRestore();
            UnlockParryWithMirage();
        }

        #region Unlock
        
        public bool IsParryUnlocked() => _parryUnlocked;

        private void UnlockParry() 
            => TryUnlock(parryUnlockButton, ref _parryUnlocked);
        
        private void UnlockRestore() 
            => TryUnlock(restoreUnlockButton, ref _restoreUnlocked);
        
        private void UnlockParryWithMirage() 
            => TryUnlock(parryWithMirageUnlockButton, ref _parryWithMirageUnlocked);

        #endregion
        
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
