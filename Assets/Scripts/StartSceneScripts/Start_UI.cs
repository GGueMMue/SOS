using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_UI : MonoBehaviour
{
    public GameObject contMenu;
    // Start is called before the first frame update
    void Start()
    {
        DataForScoreCalculator.NOW_STAGE = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClickExitEventListener()
    {
        Application.Quit();
    }

    public void EnmationContineMenu()
    {
        contMenu.SetActive(true);        
    }

    public void EnmationContineMenuExit()
    {
        contMenu.SetActive(false);
    }

    public void OnClickStartEventLitener()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void OnClickStage1Btn()
    {
        SceneManager.LoadScene("Stage1");
    }
    public void OnClickStage2Btn()
    {
        SceneManager.LoadScene("Stage2");
    }
    public void OnClickStage3Btn()
    {
        SceneManager.LoadScene("Stage3");
    }
    public void OnClickStage4Btn()
    {
        SceneManager.LoadScene("Stage4");
    }
}
