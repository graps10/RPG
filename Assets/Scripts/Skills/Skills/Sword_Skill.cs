using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}
public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce Info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    public bool swordUnlocked {get; private set;}
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce Info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin Info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown = 0.35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;


    [Header("Skill Info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive Skills")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked {get; private set;}
    [SerializeField] private UI_SkillTreeSlot volnerableUnlockButton;
    public bool volnerableUnlocked {get; private set;}

    private Vector2 finalDir;

    [Header("Aim Dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void Start()
    {
        base.Start();

        GenerateDots();
        SetupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);

        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        volnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVolnerable);
    }

    private void SetupGravity()
    {
        if(swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if(Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i]. transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if(swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
        
        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Unlock region

    private void UnlockTimeStop()
    {
        if(timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
    }

    private void UnlockVolnerable()
    {
        if(volnerableUnlockButton.unlocked)
            volnerableUnlocked = true;
    }

    private void UnlockSword()
    {
        if(swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }    
    }

    private void UnlockBounceSword()
    {
        if(bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if(pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if(spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }

    #endregion

    #region Aim Region

    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePostion - playerPosition;

        return direction;
    }

    public void DotsActive(bool isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(isActive);
        }
    }
    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }
    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x, 
            AimDirection().normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity * (t* t));

        return position;
    }
    
    #endregion
}
