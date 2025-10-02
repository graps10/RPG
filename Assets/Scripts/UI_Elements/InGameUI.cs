using Items_and_Inventory;
using Managers;
using Skills;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Elements
{
    public class InGameUI : MonoBehaviour
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

        private SkillManager _skills;

        private void Start()
        {
            if (playerStats != null)
                playerStats.OnHealthChanged += UpdateHealthUI;

            _skills = SkillManager.Instance;
        }

        private void Update()
        {
            UpdateSoulsUI();

            UpdateSkillCooldownUI(dashImage, 
                _skills.Dash.GetCooldownRemaining(), _skills.Dash.GetCooldownDuration());
            UpdateSkillCooldownUI(parryImage, 
                _skills.Parry.GetCooldownRemaining(), _skills.Parry.GetCooldownDuration());
            UpdateSkillCooldownUI(crystalImage, 
                _skills.Crystal.GetCooldownRemaining(), _skills.Crystal.GetCooldownDuration());
            UpdateSkillCooldownUI(swordImage, 
                _skills.Sword.GetCooldownRemaining(), _skills.Sword.GetCooldownDuration());
            UpdateSkillCooldownUI(blackHoleImage, 
                _skills.BlackHole.GetCooldownRemaining(), _skills.BlackHole.GetCooldownDuration());
            
            if (Input.GetKeyDown(KeyCode.E) && Inventory.Instance.GetEquippedItem(EquipmentType.Flask) != null)
                SetCooldownOf(flaskImage);

            CheckCooldownOf(flaskImage, Inventory.Instance.GetEquipmentCooldowns().GetFlaskCooldown());
        }

        private void UpdateSoulsUI()
        {
            if (soulsAmount < PlayerManager.Instance.GetCurrency())
                soulsAmount += Time.deltaTime * increaseRate;
            else
                soulsAmount = PlayerManager.Instance.GetCurrency();

            currentSouls.text = ((int)soulsAmount).ToString();
        }

        private void UpdateHealthUI()
        {
            slider.maxValue = playerStats.GetMaxHealthValue();
            slider.value = playerStats.CurrentHealth;
        }

        private void UpdateSkillCooldownUI(Image image, float currentTimer, float maxCooldown)
        {
            if (currentTimer > 0f)
                image.fillAmount = currentTimer / maxCooldown;
            else
                image.fillAmount = 0f;
        }

        private void SetCooldownOf(Image image)
        {
            if (image.fillAmount <= 0f)
                image.fillAmount = 1f;
        }

        private void CheckCooldownOf(Image image, float cooldown)
        {
            if (image.fillAmount > 0f)
                image.fillAmount -= 1f / cooldown * Time.deltaTime;
        }
    }
}
