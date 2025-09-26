using Controllers.Skill_Controllers;
using Managers;
using UI_Elements;
using UnityEngine;
using UnityEngine.UI;

namespace Skills.Skills
{
    public class BlackHoleSkill : Skill
    {
        [SerializeField] private GameObject blackHolePrefab;
        [SerializeField] private SkillTreeSlot blackHoleUnlockButton;
        
        [SerializeField] private int amountOfAttacks = 4;
        [SerializeField] private float cloneCooldown = 0.3f;
        [SerializeField] private float blackHoleDuration;
        [Space]
        [SerializeField] private float maxSize;
        [SerializeField] private float growSpeed;
        [SerializeField] private float shrinkSpeed;

        private bool _blackHoleUnlocked;
        private BlackHoleSkillController _currentBlackHole;
        
        private void OnEnable()
        {
            blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(blackHoleUnlockButton, ref _blackHoleUnlocked));
        }

        private void OnDisable()
        {
            if(blackHoleUnlockButton != null)
                blackHoleUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        protected override void CheckUnlock()
        {
            TryUnlock(blackHoleUnlockButton, ref _blackHoleUnlocked);
        }
        
        public bool IsBlackHoleUnlocked() => _blackHoleUnlocked;

        public override void UseSkill()
        {
            base.UseSkill();

            GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

            _currentBlackHole = newBlackHole.GetComponent<BlackHoleSkillController>();

            _currentBlackHole.SetupBlackHole(
                maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackHoleDuration);

            AudioManager.Instance.PlaySFX(4, player.transform);
            AudioManager.Instance.PlaySFX(5, player.transform);
        }

        public bool SkillCompleted()
        {
            if (!_currentBlackHole) return false;

            if (_currentBlackHole.PlayerCanExitState)
            {
                _currentBlackHole = null;
                return true;
            }

            return false;
        }

        public float GetBlackHoleRadius() => maxSize / 2;
    }
}
