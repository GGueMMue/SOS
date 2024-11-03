using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Score_Board_UiManager : MonoBehaviour
{
    public void OnClickToMainEventListener()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OnClickNextStageEventListner()
    {
        Debug.Log("Current Stage: " + DataForScoreCalculator.NOW_STAGE);

        switch (DataForScoreCalculator.NOW_STAGE)
        {
            case "Stage1":
                Debug.Log("Loading Stage2");
                SceneManager.LoadScene("Stage2");
                break;

            case "Stage2":
                Debug.Log("Loading Stage3");
                SceneManager.LoadScene("Stage3");
                break;

            case "Stage3":
                Debug.Log("Loading Stage4");
                SceneManager.LoadScene("Stage4");
                break;

            case "Stage4":
                Debug.Log("Loading StartScene");
                SceneManager.LoadScene("StartScene");
                break;

            default:
                Debug.Log("Unknown stage: " + DataForScoreCalculator.NOW_STAGE);
                break;
        }
    }
}
