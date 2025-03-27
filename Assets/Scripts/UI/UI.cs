using System.Collections;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    public UI_SkillTooltip skillTooltip;
    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;
    public UI_CraftWindow craftWindow;

    void Awake()
    {
        SwitchTo(skillTreeUI); // to assign events on skill tree slots before we assign events on skill scripts
        fadeScreen.gameObject.SetActive(true);
    }

    void Start() 
    {
        SwitchTo(inGameUI);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(characterUI);
        
        if(Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);
        
        if(Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if(Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);
    }
    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null; // to keep fade screen object active
            
            if(!fadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if(_menu != null)
            _menu.SetActive(true);
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if(_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        HideTootTips();
        SwitchTo(_menu);  
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();

        StartCoroutine(EndScreenCoroutine());
    }

    public void RestartGameButton() => GameManager.instance.RestartScene();

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
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

    private IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1f);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }
}
