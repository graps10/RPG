using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked {get; private set;}

    [Header("Mirage Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodgeButton;
    public bool dodgeMirageUnlocked {get; private set;}

    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    public void CreateMirageOnDodge()
    {
        if(dodgeMirageUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir, 0));
    }

    private void UnlockDodge()
    {
        if(unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        } 
    }

    private void UnlockMirageDodge()
    {
        if(unlockMirageDodgeButton.unlocked)
            dodgeMirageUnlocked = true;
    }
}
