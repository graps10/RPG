using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("Teleport Settings")]
    [SerializeField] private float teleportCooldown = 0.5f;
    private bool canTeleport = true;

    [Header("Crystal Mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal;


    [Header("Explosive Crystal")]
    [SerializeField] private bool canExplode;


    [Header("Moving Crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi Stacking Crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalsLeft = new List<GameObject>();

    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal()) return;

        if(currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if(canMoveToEnemy || !canTeleport) return;

            PerformTeleport();
            
            if(cloneInsteadOfCrystal)
                HandleCloneCreation();
            else
                HandleCrystalFinish();
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        
        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>()?.ChooseRandomEnemy();

    private void PerformTeleport()
    {
        Vector2 playerPos = player.transform.position;
        player.transform.position = currentCrystal.transform.position;
        currentCrystal.transform.position = playerPos;

        LockTeleport();
    }
    private void HandleCloneCreation()
    {
        SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
        Destroy(currentCrystal);

        Invoke(nameof(UnlockTeleport), teleportCooldown);
    }
    private void HandleCrystalFinish()
    {
        currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
        
        Invoke(nameof(UnlockTeleport), teleportCooldown);
    }

    private bool CanUseMultiCrystal()
    {
        if(canUseMultiStacks)
        {
            if(crystalsLeft.Count > 0)
            {
                if(crystalsLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);
                
                cooldown = 0;

                GameObject crystalToSpawn = crystalsLeft[crystalsLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
            
                crystalsLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);
            
                if(crystalsLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }
                return true;
            }
        }

        return false;
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalsLeft.Count;
        
        for (int i = 0; i < amountToAdd; i++)
        {
            crystalsLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if(cooldownTimer > 0) return;

        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }

    private void UnlockTeleport() => canTeleport = true;
    private void LockTeleport() => canTeleport = false;
}
