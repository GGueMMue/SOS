using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text bulletText;
    [SerializeField] bool isGunNull = true;
    Gun gun;
    
    //public Canvas canvas;

    public Text alert_Score_Text_Board;

    // Start is called before the first frame update
    void Start()
    {

    }

  

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Gun>() != null)
        {
            gun = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Gun>();

            if (gun.gunName != "Meele")
            {
                isGunNull = true;
            }
            else isGunNull= false;
        }

        else
        {
            isGunNull = false;
        }

        if(isGunNull)
        {
            bulletText.text = gun.curBullet + " / " + gun.maxReroadableBullet;
            
        }
        else
        {
            bulletText.text =  "- / -" ;
        }
    }

    public void PrintAlertScoreBoard(int score, Transform tr)
    {
        Text text = Instantiate(alert_Score_Text_Board, GameObject.FindGameObjectWithTag("Canvas_Tag").transform);
        text.text = "+ " + score;

        //text.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Tag").transform);

        Vector3 textPos = Camera.main.WorldToScreenPoint(tr.position + new Vector3(0, 1, 0));

        text.transform.position = textPos;
        text.transform.rotation = Quaternion.identity;

        Destroy(text, .5f);        
    } // 업데이트에서 진행해야 할 것 같음. 실시간으로 움직이질 않음.
}
