using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_UI : MonoBehaviour
{
    public GameObject contMenu;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
