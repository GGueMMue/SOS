using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Verdict : MonoBehaviour
{
    // 현재 미 사용 중인 스크립트
    [SerializeField]                            Player_Controller ps;

    private void Start()
    {
       ps = GetComponentInParent<Player_Controller>();
    }

    public void OnTriggerStay(Collider other)
    {
        Debug.Log(other.tag);
        if (other.gameObject.GetComponent<FSM>().state == FSM.STATE.DEAD)
        {
            //Destroy(other.gameObject);
            ps.possible_Kill_Confirm = true;
        }
    }
}
