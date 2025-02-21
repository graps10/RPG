using UnityEngine;
using UnityEngine.UI;

public class BlackHole_Skill : Skill
{
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackHoleUnlocked {get; private set;
    }
    [SerializeField] private int amountOfAttacks = 4;
    [SerializeField] private float cloneCooldown = 0.3f;
    [SerializeField] private float blackHoleDuration;
    [Space]
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    BlackHole_Skill_Controller currentBlackHole;
    
    protected override void Start()
    {
        base.Start();

        blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackHole);
    }
    protected override void Update()
    {
        base.Update();
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }
    
    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        currentBlackHole = newBlackHole.GetComponent<BlackHole_Skill_Controller>();

        currentBlackHole.SetupBlackHole(
            maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackHoleDuration);
    }

    public bool SkillCompleted()
    {
        if(!currentBlackHole) return false;

        if(currentBlackHole.playerCanExitState)
        {
            currentBlackHole = null;
            return true;
        }

        return false;
    }

    public float GetBlackHoleRadius() => maxSize / 2;

    private void UnlockBlackHole()
    {
        if(blackHoleUnlockButton.unlocked)
            blackHoleUnlocked = true;
    }
}
