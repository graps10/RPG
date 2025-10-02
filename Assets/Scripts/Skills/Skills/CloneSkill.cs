using System.Collections;
using Controllers.Skill_Controllers;
using UI_Elements;
using UnityEngine;

namespace Skills.Skills
{
    public class CloneSkill : Skill
    {
        private const float Clone_Delay = 0.4f;
        
        private static readonly Vector3 cloneOffset = new(2f, 0f, 0f);
    
        [Header("Clone Info")]
        [SerializeField] private float attackMultiplier;
        [SerializeField] private GameObject clonePrefab;
        [SerializeField] private float cloneDuration;
        [Space]

        [Header("Clone Attack")]
        [SerializeField] private SkillTreeSlot cloneAttackUnlockButton;
        [SerializeField] private float cloneAttackMultiplier;
        [SerializeField] private bool canAttack;

        [Header("Aggressive clone")]
        [SerializeField] private SkillTreeSlot aggressiveCloneUnlockButton;
        [SerializeField] private float aggressiveCloneAttackMultiplier;
    
        [Header("Multiple clone")]
        [SerializeField] private SkillTreeSlot multipleUnlockButton;
        [SerializeField] private float multiCloneAttackMultiplier;
        [SerializeField] private bool canDuplicateClone;
        [SerializeField] private float chanceToDuplicate;

        [Header("Crystal instead of clone")]
        [SerializeField] private SkillTreeSlot crystalInsteadUnlockButton;

        private bool _canApplyOnHitEffect;
        private bool _crystalInsteadOfClone;

        private void OnEnable()
        {
            cloneAttackUnlockButton.OnUnlocked += UnlockCloneCanAttack;
            aggressiveCloneUnlockButton.OnUnlocked += UnlockAggressiveClone;
            multipleUnlockButton.OnUnlocked += UnlockMultipleClone;
            crystalInsteadUnlockButton.OnUnlocked += UnlockCrystalInstead;
        }

        private void OnDisable()
        {
            if(cloneAttackUnlockButton != null)
                cloneAttackUnlockButton.OnUnlocked -= UnlockCloneCanAttack;
            
            if(aggressiveCloneUnlockButton != null)
                aggressiveCloneUnlockButton.OnUnlocked -= UnlockAggressiveClone;
            
            if(multipleUnlockButton != null)
                multipleUnlockButton.OnUnlocked -= UnlockMultipleClone;
            
            if(crystalInsteadUnlockButton != null)
                crystalInsteadUnlockButton.OnUnlocked -= UnlockCrystalInstead;
        }

        protected override void CheckUnlock()
        { 
            UnlockCloneCanAttack();
            UnlockAggressiveClone();
            UnlockMultipleClone();
            UnlockCrystalInstead();
        }

        # region Unlock
        private void UnlockCloneCanAttack() 
            => TryUnlock(cloneAttackUnlockButton, ref canAttack, 
                () =>  attackMultiplier = cloneAttackMultiplier);
        
        private void UnlockAggressiveClone() 
            => TryUnlock(aggressiveCloneUnlockButton, ref _canApplyOnHitEffect, 
                () => attackMultiplier = aggressiveCloneAttackMultiplier);
        
        private void UnlockMultipleClone() 
            => TryUnlock(multipleUnlockButton, ref canDuplicateClone, 
                () =>  attackMultiplier = multiCloneAttackMultiplier);
        
        private void UnlockCrystalInstead()
            => TryUnlock(crystalInsteadUnlockButton, ref _crystalInsteadOfClone);
        
        #endregion
        
        public void CreateClone(Transform clonePosition, Vector3 offset)
        {
            if (_crystalInsteadOfClone)
            {
                SkillManager.Instance.Crystal.CreateCrystal();
                return;
            }

            GameObject newClone = Instantiate(clonePrefab);
            newClone.GetComponent<CloneSkillController>().
                SetupClone(clonePosition, cloneDuration, canAttack, offset, 
                    canDuplicateClone, chanceToDuplicate, player, attackMultiplier);
        }

        public void CreateCloneWithDelay(Transform enemyTransform)
        {
            StartCoroutine(CloneDelayCoroutine(enemyTransform, 
                new Vector3(cloneOffset.x * player.FacingDir, cloneOffset.y, cloneOffset.z)));
        }

        private IEnumerator CloneDelayCoroutine(Transform transform, Vector3 offset)
        {
            yield return new WaitForSeconds(Clone_Delay);
            CreateClone(transform, offset);
        }
        
        public bool CanApplyOnHitEffect() => _canApplyOnHitEffect;
        public bool IsCrystalInsteadOfClone() => _crystalInsteadOfClone;
    }
}
