using Components.Audio;
using Controllers.Skill_Controllers;
using Core.ObjectPool.Configs.Controllers;
using Managers;
using UI_Elements;
using UnityEngine;

namespace Skills.Skills
{
    public class BlackHoleSkill : Skill
    {
        [SerializeField] private BlackHolePoolConfig blackHoleConfig;
        [SerializeField] private SkillTreeSlot blackHoleUnlockButton;

        private bool _blackHoleUnlocked;
        private BlackHoleSkillController _currentBlackHole;
        
        private void OnEnable()
        {
            blackHoleUnlockButton.OnUnlocked += UnlockBlackHole;
        }

        private void OnDisable()
        {
            if(blackHoleUnlockButton != null)
                blackHoleUnlockButton.OnUnlocked -= UnlockBlackHole;
        }

        protected override void CheckUnlock()
        {
            UnlockBlackHole();
        }

        private void UnlockBlackHole()
        {
            TryUnlock(blackHoleUnlockButton, ref _blackHoleUnlocked);
        }

        public override void UseSkill()
        {
            base.UseSkill();

            GameObject newBlackHole = Core.ObjectPool.PoolManager.Instance.Spawn(
                blackHoleConfig.Prefab, 
                player.transform.position, 
                Quaternion.identity
            );

            if (newBlackHole.TryGetComponent(out _currentBlackHole))
                _currentBlackHole.SetupBlackHole(blackHoleConfig);

            AudioManager.Instance.PlaySFX(SFXEnum.Bankai, player.transform);
            AudioManager.Instance.PlaySFX(SFXEnum.Chronosphere, player.transform);
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
        
        public bool IsBlackHoleUnlocked() => _blackHoleUnlocked;
        public float GetBlackHoleRadius() => blackHoleConfig.MaxSize / 2;
    }
}
