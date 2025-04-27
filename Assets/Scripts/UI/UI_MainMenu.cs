using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UI_FadeScreen fadeScreen;

    // void Start()
    // {
    //     if(SaveManager.instance.HasSavedData() == false)
    //         continueButton.SetActive(false);
    // }

    public void PlayGame() => StartCoroutine(LoadSceneWithFadeEffect(1.5f));

    public void ContinueGame() => StartCoroutine(LoadSceneWithFadeEffect(1.5f));

    public void NewGame()
    {
        SaveManager.instance.DeleteSavedData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void ExitGame() => Application.Quit();

    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);
    }
}
