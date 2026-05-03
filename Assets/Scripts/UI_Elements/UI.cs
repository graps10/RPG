using System.Collections;
using Components.Audio;
using Core.Save_and_Load;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI_Elements
{
    public class UI : MonoBehaviour
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

        // The "Save & Exit" button lives on the options panel but drives scene management,
        // so it stays here in UI rather than in OptionsUI.
        [SerializeField] private Button saveAndExitButton;

        private void OnEnable()
        {
            if (saveAndExitButton != null)
                saveAndExitButton.onClick.AddListener(SaveAndExit);
        }

        private void OnDisable()
        {
            if (saveAndExitButton != null)
                saveAndExitButton.onClick.RemoveListener(SaveAndExit);
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

        #endregion
    }
}

