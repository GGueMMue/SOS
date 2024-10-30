using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnManager : MonoBehaviour
{
    public void OnClickToMainMenuBtnEvent()
    {
        SceneManager.LoadScene("");
    }

    public void OnClickQuitBtnEvent()
    {
        Application.Quit();
    }
}
