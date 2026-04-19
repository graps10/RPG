using System.Collections;
using System.Collections.Generic;
using Components.Audio;
using Core.Save_and_Load;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI_Elements
{
    public class UI : MonoBehaviour, ISaveManager
    {
        private const float End_Text_Delay = 1f;
        private const float Restart_Button_Delay = 1.5f;
        
        private const float Load_Scene_Fade_Duration = 0f;
        
        [Header("End Screen")] 
        [SerializeField] private FadeScreen fadeScreen;

        [SerializeField] private GameObject endText;
        [SerializeField] private GameObject restartButton;
        [Space] [SerializeField] private GameObject characterUI;
        [SerializeField] private GameObject skillTreeUI;
        [SerializeField] private GameObject craftUI;
        [SerializeField] private GameObject optionsUI;
        [SerializeField] private GameObject inGameUI;

        [SerializeField] private SkillTooltip skillTooltip;
        [SerializeField] private ItemTooltip itemTooltip;
        [SerializeField] private StatTooltip statTooltip;
        [SerializeField] private CraftWindow craftWindow;
        
        private void Awake()
        {
            SwitchTo(skillTreeUI); // to assign events on skill tree slots before we assign events on skill scripts
            fadeScreen.gameObject.SetActive(true);
        }

        private void Start()
        {
            SwitchTo(inGameUI);

            itemTooltip.gameObject.SetActive(false);
            statTooltip.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                SwitchWithKeyTo(characterUI);

            if (Input.GetKeyDown(KeyCode.B))
                SwitchWithKeyTo(craftUI);

            if (Input.GetKeyDown(KeyCode.K))
                SwitchWithKeyTo(skillTreeUI);

            if (Input.GetKeyDown(KeyCode.Escape))
                SwitchWithKeyTo(optionsUI);
        }

        public SkillTooltip GetSkillTooltip() => skillTooltip;
        public ItemTooltip GetItemTooltip() => itemTooltip;
        public StatTooltip GetStatTooltip() => statTooltip;
        public CraftWindow GetCraftWindow() => craftWindow;

        public void SwitchTo(GameObject menu)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                bool fadeScreen =
                    transform.GetChild(i).GetComponent<FadeScreen>() != null; // to keep fade screen object active

                if (!fadeScreen)
                    transform.GetChild(i).gameObject.SetActive(false);
            }

            if (menu != null)
            {
                AudioManager.Instance.PlaySFX(SFXEnum.Click);
                menu.SetActive(true);
            }

            if (GameManager.Instance != null)
            {
                GameManager.PauseGame(menu != inGameUI);
            }
        }

        public void SwitchWithKeyTo(GameObject menu)
        {
            if (menu != null && menu.activeSelf)
            {
                menu.SetActive(false);
                CheckForInGameUI();
                return;
            }

            HideTootTips();
            SwitchTo(menu);
        }

        public void SwitchOnEndScreen()
        {
            fadeScreen.FadeOut();
            StartCoroutine(EndScreenRoutine());
        }

        public void RestartGameButton() => GameManager.RestartScene();

        private void CheckForInGameUI()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf &&
                    transform.GetChild(i).GetComponent<FadeScreen>() == null)
                    return;
            }

            SwitchTo(inGameUI);
        }

        private void HideTootTips()
        {
            if (skillTooltip.gameObject.activeSelf)
                skillTooltip.HideToolTip();

            if (itemTooltip.gameObject.activeSelf)
                itemTooltip.HideToolTip();

            if (statTooltip.gameObject.activeSelf)
                statTooltip.HideStatToolTip();
        }

        private IEnumerator EndScreenRoutine()
        {
            yield return new WaitForSeconds(End_Text_Delay);
            endText.SetActive(true);

            yield return new WaitForSeconds(Restart_Button_Delay);
            restartButton.SetActive(true);
        }

        #region Options

        [SerializeField] private VolumeSlider[] volumeSettings;
        [SerializeField] private Toggle showHealthBarToggle;
        [SerializeField] private Button saveAndExitButton;
        
        public static bool ShowHealthBar { get; private set; } = true;

        private void OnEnable()
        {
            if (showHealthBarToggle != null)
                showHealthBarToggle.onValueChanged.AddListener(TogglePlayerHealthBar);
            
            if (saveAndExitButton != null)
                saveAndExitButton.onClick.AddListener(SaveAndExit);
        }

        private void OnDisable()
        {
            if (showHealthBarToggle != null)
                showHealthBarToggle.onValueChanged.RemoveListener(TogglePlayerHealthBar);
            
            if (saveAndExitButton != null)
                saveAndExitButton.onClick.RemoveListener(SaveAndExit);
        }

        private void TogglePlayerHealthBar(bool isOn)
        {
            ShowHealthBar = isOn;
            
            if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerGameObject != null)
            {
                HealthBar playerHealthBar =
                    PlayerManager.Instance.PlayerGameObject.GetComponentInChildren<HealthBar>(true);

                if (playerHealthBar != null)
                    playerHealthBar.gameObject.SetActive(isOn);
            }
        }

        private void SaveAndExit()
        {
            SaveManager.Instance.SaveGame();
            StartCoroutine(LoadMainMenuCoroutine());
        }

        private IEnumerator LoadMainMenuCoroutine()
        {
            fadeScreen.FadeIn();
            
            yield return new WaitForSecondsRealtime(Load_Scene_Fade_Duration); 
            
            if (GameManager.Instance != null)
                GameManager.PauseGame(false);
            
            SceneManager.LoadScene("MainMenu");
        }

        public void LoadData(GameData data)
        {
            ShowHealthBar = data.ShowPlayerHealthBar;

            if (showHealthBarToggle != null)
            {
                showHealthBarToggle.onValueChanged.RemoveListener(TogglePlayerHealthBar);
                showHealthBarToggle.isOn = ShowHealthBar;
                showHealthBarToggle.onValueChanged.AddListener(TogglePlayerHealthBar);
            }

            foreach (KeyValuePair<string, float> pair in data.GetVolumeSettings())
            {
                foreach (VolumeSlider item in volumeSettings)
                {
                    if (item.GetParameter() == pair.Key)
                    {
                        item.LoadSlider(pair.Value);

                        if (item.GetParameter() == AudioManager.MIXER_BGM)
                            AudioManager.Instance.SetupBGMVolume(pair.Value);
                        else
                            AudioManager.Instance.SetupSFXVolume(pair.Value);
                    }
                }
            }
        }

        public void SaveData(ref GameData data)
        {
            data.SetShowPlayerHealthBar(ShowHealthBar);

            data.GetVolumeSettings().Clear();

            foreach (VolumeSlider item in volumeSettings)
            {
                data.GetVolumeSettings().Add(item.GetParameter(), item.GetSlider().value);
            }
        }

        #endregion
    }
}

