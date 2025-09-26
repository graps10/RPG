using System.Collections.Generic;
using Controllers.Skill_Controllers;
using UI_Elements;
using UnityEngine;
using UnityEngine.UI;

namespace Skills.Skills
{
    public class CrystalSkill : Skill
    {
        [SerializeField] private GameObject crystalPrefab;
        [SerializeField] private float crystalDuration;
        
        [Header("Crystal Skill")]
        [SerializeField] private SkillTreeSlot unlockCrystalButton;

        [Header("Teleport Settings")]
        [SerializeField] private float teleportCooldown = 0.5f;
        
        [Header("Crystal Mirage")]
        [SerializeField] private SkillTreeSlot unlockCloneInsteadButton;
        [SerializeField] private bool cloneInsteadOfCrystal;
        
        [Header("Explosive Crystal")]
        [SerializeField] private SkillTreeSlot unlockExplosiveButton;
        [SerializeField] private bool canExplode;
        
        [Header("Moving Crystal")]
        [SerializeField] private SkillTreeSlot unlockMovingCrystalButton;
        [SerializeField] private bool canMoveToEnemy;
        [SerializeField] private float moveSpeed;

        [Header("Multi Stacking Crystal")]
        [SerializeField] private SkillTreeSlot unlockMultiStackButton;
        [SerializeField] private bool canUseMultiStacks;
        [SerializeField] private int amountOfStacks;
        [SerializeField] private float multiStackCooldown;
        [SerializeField] private float useTimeWindow;
        [SerializeField] private List<GameObject> crystalsLeft = new();

        private bool _crystalUnlocked;
        
        private GameObject _currentCrystal;
        private bool _canTeleport = true;

        private void OnEnable()
        {
            unlockCrystalButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(unlockCrystalButton, ref _crystalUnlocked));
            
            unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(unlockCloneInsteadButton, ref cloneInsteadOfCrystal));
            
            unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(unlockExplosiveButton, ref canExplode));
            
            unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(
                () =>  TryUnlock(unlockMovingCrystalButton, ref canMoveToEnemy));
            
            unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(
                () =>  TryUnlock(unlockMultiStackButton, ref canUseMultiStacks));
        }

        private void OnDisable()
        {
            if(unlockCrystalButton != null)
                unlockCrystalButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(unlockCloneInsteadButton != null)
                unlockCloneInsteadButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(unlockExplosiveButton != null)
                unlockExplosiveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(unlockMovingCrystalButton != null)
                unlockMovingCrystalButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(unlockMultiStackButton != null)
                unlockMultiStackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        public override void UseSkill()
        {
            base.UseSkill();

            if (CanUseMultiCrystal()) return;

            if (_currentCrystal == null)
            {
                CreateCrystal();
            }
            else
            {
                if (canMoveToEnemy || !_canTeleport) return;

                PerformTeleport();

                if (cloneInsteadOfCrystal)
                    HandleCloneCreation();
                else
                    HandleCrystalFinish();
            }
        }

        public void CreateCrystal()
        {
            _currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            CrystalSkillController currentCrystalScript = 
                _currentCrystal.GetComponent<CrystalSkillController>();

            currentCrystalScript.SetupCrystal(crystalDuration, canExplode, 
                canMoveToEnemy, moveSpeed, FindClosestEnemy(_currentCrystal.transform), player);
        }

        public void CurrentCrystalChooseRandomTarget() 
            => _currentCrystal.GetComponent<CrystalSkillController>()?.ChooseRandomEnemy();
        
        protected override void CheckUnlock()
        {
            TryUnlock(unlockCrystalButton, ref _crystalUnlocked);
            TryUnlock(unlockCloneInsteadButton, ref cloneInsteadOfCrystal);
            TryUnlock(unlockExplosiveButton, ref canExplode);
            TryUnlock(unlockMovingCrystalButton, ref canMoveToEnemy);
            TryUnlock(unlockMultiStackButton, ref canUseMultiStacks);
        }

        public bool IsCrystalUnlocked() => _crystalUnlocked;
        
        private void PerformTeleport()
        {
            Vector2 playerPos = player.transform.position;
            player.transform.position = _currentCrystal.transform.position;
            _currentCrystal.transform.position = playerPos;

            LockTeleport();
        }

        private void HandleCloneCreation()
        {
            SkillManager.Instance.Clone.CreateClone(_currentCrystal.transform, Vector3.zero);
            Destroy(_currentCrystal);

            Invoke(nameof(UnlockTeleport), teleportCooldown);
        }

        private void HandleCrystalFinish()
        {
            _currentCrystal.GetComponent<CrystalSkillController>()?.FinishCrystal();

            Invoke(nameof(UnlockTeleport), teleportCooldown);
        }

        private bool CanUseMultiCrystal()
        {
            if (canUseMultiStacks)
            {
                if (crystalsLeft.Count > 0)
                {
                    if (crystalsLeft.Count == amountOfStacks)
                        Invoke(nameof(ResetAbility), useTimeWindow);

                    SetCooldownDuration(0f);

                    GameObject crystalToSpawn = crystalsLeft[^1];
                    GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                    crystalsLeft.Remove(crystalToSpawn);
                    newCrystal.GetComponent<CrystalSkillController>().
                        SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, 
                            FindClosestEnemy(newCrystal.transform), player);

                    if (crystalsLeft.Count <= 0)
                    {
                        SetCooldownDuration(multiStackCooldown);
                        RefillCrystal();
                    }
                    return true;
                }
            }

            return false;
        }

        private void RefillCrystal()
        {
            int amountToAdd = amountOfStacks - crystalsLeft.Count;

            for (int i = 0; i < amountToAdd; i++)
            {
                crystalsLeft.Add(crystalPrefab);
            }
        }

        private void ResetAbility()
        {
            if (cooldownRemaining > 0) return;

            cooldownRemaining = multiStackCooldown;
            RefillCrystal();
        }

        private void UnlockTeleport() => _canTeleport = true;
        private void LockTeleport() => _canTeleport = false;
    }
}
