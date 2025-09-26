using UI_Elements;
using UnityEngine;
using UnityEngine.UI;

namespace Skills.Skills
{
    public class DashSkill : Skill
    {
        [Header("Dash")]
        [SerializeField] private SkillTreeSlot dashUnlockButton;
        
        [Header("Clone on dash")]
        [SerializeField] private SkillTreeSlot cloneOnDashUnlockButton;
        
        [Header("Clone on arrival")]
        [SerializeField] private SkillTreeSlot cloneOnArrivalUnlockButton;

        private bool _dashUnlocked;
        private bool _cloneOnDashUnlocked;
        private bool _cloneOnArrivalUnlocked;

        private void OnEnable()
        {
            dashUnlockButton.OnUnlocked += UnlockDash;
            cloneOnDashUnlockButton.OnUnlocked += UnlockCloneOnDash;
            cloneOnArrivalUnlockButton.OnUnlocked += UnlockCloneOnArrival;
        }

        private void OnDisable()
        {
            if(dashUnlockButton != null)
                dashUnlockButton.OnUnlocked -= UnlockDash;
            
            if(cloneOnDashUnlockButton != null)
                cloneOnDashUnlockButton.OnUnlocked -= UnlockCloneOnDash;
            
            if(cloneOnArrivalUnlockButton)
                cloneOnArrivalUnlockButton.OnUnlocked -= UnlockCloneOnArrival;
        }

        protected override void CheckUnlock()
        {
            UnlockDash();
            UnlockCloneOnDash();
            UnlockCloneOnArrival();
        }

        #region Unlock

        public bool IsDashUnlocked() => _dashUnlocked;
        
        private void UnlockDash() 
            => TryUnlock(dashUnlockButton, ref _dashUnlocked);
        
        private void UnlockCloneOnDash() 
            => TryUnlock(cloneOnDashUnlockButton, ref _cloneOnDashUnlocked);
        
        private void UnlockCloneOnArrival() 
            => TryUnlock(cloneOnArrivalUnlockButton, ref _cloneOnArrivalUnlocked);

        #endregion

        public void CloneOnDash()
        {
            if (_cloneOnDashUnlocked)
                SkillManager.Instance.Clone.CreateClone(player.transform, Vector3.zero);
        }

        public void CloneOnArrival()
        {
            if (_cloneOnArrivalUnlocked)
                SkillManager.Instance.Clone.CreateClone(player.transform, Vector3.zero);
        }
    }
}
