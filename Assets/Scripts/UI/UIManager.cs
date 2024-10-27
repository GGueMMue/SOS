using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text bulletText;
    [SerializeField] bool isGunNull = true;
    Gun gun;
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
}
