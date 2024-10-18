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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Finder.SetActive(false);
            fsm.canShot = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Finder.SetActive(true);
            fsm.canShot = false;
        }
    }
}
