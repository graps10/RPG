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
            dashUnlockButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(dashUnlockButton, ref _dashUnlocked));
            
            cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(cloneOnDashUnlockButton, ref _cloneOnDashUnlocked));
            
            cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(
                () =>  TryUnlock(cloneOnArrivalUnlockButton, ref _cloneOnArrivalUnlocked));
        }

        private void OnDisable()
        {
            if(dashUnlockButton != null)
                dashUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(cloneOnDashUnlockButton != null)
                cloneOnDashUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(cloneOnArrivalUnlockButton)
                cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        protected override void CheckUnlock()
        {
            TryUnlock(dashUnlockButton, ref _dashUnlocked);
            TryUnlock(cloneOnDashUnlockButton, ref _cloneOnDashUnlocked);
            TryUnlock(cloneOnArrivalUnlockButton, ref _cloneOnArrivalUnlocked);
        }
        
        public bool IsDashUnlocked() => _dashUnlocked;

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
