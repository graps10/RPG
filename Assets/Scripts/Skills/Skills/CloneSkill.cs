using System.Collections;
using Controllers.Skill_Controllers;
using UI_Elements;
using UnityEngine;
using UnityEngine.UI;

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
            cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(
                () =>  TryUnlock(cloneAttackUnlockButton, ref canAttack, 
                    () =>  attackMultiplier = cloneAttackMultiplier));
            
            aggressiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(aggressiveCloneUnlockButton, ref _canApplyOnHitEffect, 
                    () => attackMultiplier = aggressiveCloneAttackMultiplier));
            
            multipleUnlockButton.GetComponent<Button>().onClick.AddListener(
                () => TryUnlock(multipleUnlockButton, ref canDuplicateClone, 
                    () =>  attackMultiplier = multiCloneAttackMultiplier));
            
            crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(
                () =>  TryUnlock(crystalInsteadUnlockButton, ref _crystalInsteadOfClone));
        }

        private void OnDisable()
        {
            if(cloneAttackUnlockButton != null)
                cloneAttackUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(aggressiveCloneUnlockButton != null)
                aggressiveCloneUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(multipleUnlockButton != null)
                multipleUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
            
            if(crystalInsteadUnlockButton != null)
                crystalInsteadUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        protected override void CheckUnlock()
        { 
            TryUnlock(cloneAttackUnlockButton, ref canAttack, () =>  attackMultiplier = cloneAttackMultiplier);
            TryUnlock(aggressiveCloneUnlockButton, ref _canApplyOnHitEffect, () => attackMultiplier = aggressiveCloneAttackMultiplier);
            TryUnlock(multipleUnlockButton, ref canDuplicateClone, () =>  attackMultiplier = multiCloneAttackMultiplier);
            TryUnlock(crystalInsteadUnlockButton, ref _crystalInsteadOfClone);
        }

        public bool CanApplyOnHitEffect() => _canApplyOnHitEffect;
        public bool IsCrystalInsteadOfClone() => _crystalInsteadOfClone;
        
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
    }
}
