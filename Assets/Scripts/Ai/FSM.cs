using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    public enum STATE
    {
        IDLE = 0,
        IDLE_PATROL,
        ROAR,
        FIND,
        ATTACK    
    
    }

    public STATE state;

    Transform tr;
    
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case STATE.IDLE:
                

                break;
            case STATE.IDLE_PATROL:


                break;
            case STATE.ROAR:


                break;
            case STATE.FIND:


                break;
            case STATE.ATTACK:


                break;
            default:
                break;
        }





    }
}
