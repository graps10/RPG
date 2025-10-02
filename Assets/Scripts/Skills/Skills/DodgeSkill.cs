using UI_Elements;
using UnityEngine;

namespace Skills.Skills
{
    public class DodgeSkill : Skill
    {
        private const float Mirage_Spawn_Offset = 2f;
        
        [Header("Dodge")]
        [SerializeField] private SkillTreeSlot unlockDodgeButton;
        [SerializeField] private int evasionAmount;
        
        [Header("Mirage Dodge")]
        [SerializeField] private SkillTreeSlot unlockMirageDodgeButton;

        private bool _dodgeUnlocked;
        private bool _dodgeMirageUnlocked;

        private void OnEnable()
        {
            unlockDodgeButton.OnUnlocked += UnlockDodge;
            unlockMirageDodgeButton.OnUnlocked += UnlockMirageDodge;
        }

        private void OnDisable()
        {
            if(unlockDodgeButton != null)
                unlockDodgeButton.OnUnlocked -= UnlockDodge;
            
            if(unlockMirageDodgeButton != null)
                unlockMirageDodgeButton.OnUnlocked -= UnlockMirageDodge;
        }

        protected override void CheckUnlock()
        {
            UnlockDodge();
            UnlockMirageDodge();
        }

        private void UnlockDodge()
        {
            if(unlockDodgeButton.Unlocked && !_dodgeUnlocked)
            {
                player.Stats.evasion.AddModifier(evasionAmount);
                // Inventory.Instance.UpdateStatsUI();
                _dodgeUnlocked = true;
            } 
        }
        
        private void UnlockMirageDodge() 
            => TryUnlock(unlockMirageDodgeButton, ref _dodgeMirageUnlocked);
        
        public void CreateMirageOnDodge()
        {
            if(_dodgeMirageUnlocked)
                SkillManager.Instance.Clone.CreateClone(player.transform, 
                    new Vector3(Mirage_Spawn_Offset * player.FacingDir, 0));
        }
    }
}
