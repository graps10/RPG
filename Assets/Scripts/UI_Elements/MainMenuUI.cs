using System.Collections;
using Core.Save_and_Load;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI_Elements
{
    public class MainMenuUI : MonoBehaviour
    {
        private const float Load_Scene_Fade_Duration = 1.5f;
        
        [SerializeField] private string sceneName = "TestRegion";
        [SerializeField] private GameObject continueButton;
        [SerializeField] private FadeScreen fadeScreen;

        public void PlayGame()
            => StartCoroutine(LoadSceneWithFadeEffect(Load_Scene_Fade_Duration));

        public void ContinueGame() 
            => StartCoroutine(LoadSceneWithFadeEffect(Load_Scene_Fade_Duration));

        public void NewGame()
        {
            SaveManager.Instance.DeleteSavedData();
            StartCoroutine(LoadSceneWithFadeEffect(Load_Scene_Fade_Duration));
        }

        public void ExitGame() => Application.Quit();

        private IEnumerator LoadSceneWithFadeEffect(float delay)
        {
            fadeScreen.FadeOut();
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(sceneName);
        }
    }
}
