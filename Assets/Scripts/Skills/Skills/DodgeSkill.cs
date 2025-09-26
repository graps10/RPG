using Items_and_Inventory;
using UI_Elements;
using UnityEngine;
using UnityEngine.UI;

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
            unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
            
            unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(unlockMirageDodgeButton, ref _dodgeMirageUnlocked));
        }

        private void OnDisable()
        {
            if(unlockDodgeButton != null)
                unlockDodgeButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(unlockMirageDodgeButton != null)
                unlockMirageDodgeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        protected override void CheckUnlock()
        {
            UnlockDodge();
            TryUnlock(unlockMirageDodgeButton, ref _dodgeMirageUnlocked);
        }

        public void CreateMirageOnDodge()
        {
            if(_dodgeMirageUnlocked)
                SkillManager.Instance.Clone.CreateClone(player.transform, 
                    new Vector3(Mirage_Spawn_Offset * player.FacingDir, 0));
        }

        private void UnlockDodge()
        {
            if(unlockDodgeButton.Unlocked && !_dodgeUnlocked)
            {
                player.Stats.evasion.AddModifier(evasionAmount);
                Inventory.Instance.UpdateStatsUI();
                _dodgeUnlocked = true;
            } 
        }
    }
}
