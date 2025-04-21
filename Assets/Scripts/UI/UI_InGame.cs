using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackHoleImage;
    [SerializeField] private Image flaskImage;


    [Header("Souls Info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    private SkillManager skills;

    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skills = SkillManager.instance;
    }

    void Update()
    {
        UpdateSoulsUI();

        UpdateSkillCooldownUI(dashImage, skills.dash.cooldownTimer, skills.dash.cooldown);
        UpdateSkillCooldownUI(parryImage, skills.parry.cooldownTimer, skills.parry.cooldown);
        UpdateSkillCooldownUI(crystalImage, skills.crystal.cooldownTimer, skills.crystal.cooldown);
        UpdateSkillCooldownUI(swordImage, skills.sword.cooldownTimer, skills.sword.cooldown);
        UpdateSkillCooldownUI(blackHoleImage, skills.blackHole.cooldownTimer, skills.blackHole.cooldown);


        if (Input.GetKeyDown(KeyCode.E) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        CheckCooldownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    private void UpdateSoulsUI()
    {
        if (soulsAmount < PlayerManager.instance.GetCurrency())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.instance.GetCurrency();

        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.CurrentHealth;
    }

    private void UpdateSkillCooldownUI(Image image, float currentTimer, float maxCooldown)
    {
        if (currentTimer > 0)
            image.fillAmount = currentTimer / maxCooldown;
        else
            image.fillAmount = 0;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
