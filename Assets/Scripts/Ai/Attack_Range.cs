using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Attack_Range : MonoBehaviour
{

    FSM fsm;
    public GameObject Finder;
    // Start is called before the first frame update
    void Start()
    {
        fsm = GetComponentInParent<FSM>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) // 적의 공격 범위. 안에 들어 와 있을 때, FSM 상태 ATTACK
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Finder.SetActive(false);
            fsm.canShot = true;
        }
    }

    private void OnTriggerExit(Collider other) // 적의 공격 범위. 벗어나게 되면, FSM 상태 ATTACK 해제
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Finder.SetActive(true);
            fsm.canShot = false;
        }
    }
}
