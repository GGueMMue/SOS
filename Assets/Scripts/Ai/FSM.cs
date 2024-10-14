using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour
{
    public float max_Angle = 205f;
    public float min_Angle = 35f;
    public float roar_Deviation = 3.5f;

    public enum STATE
    {
        IDLE = 0,
        IDLE_PATROL,
        ROAR,
        FIND,
        ATTACK,
        DEAD
    }
    Vector3[] patrolPoints;

    int roarCount = 0;
    Vector3 roarPoints;


    float roarTimer = 0f;
    public bool isPatrol;
    public bool nowDead = false;
    public bool needNewRocation = false;

    public STATE state;

    Transform tr;

    public Enemy_Seacher es;
    public NavMeshAgent nav;

    void SetRoarDestination()
    {
        //if(roarCount < 4)
        //{
        roarCount++;

        nav.isStopped = true;

        transform.Rotate(0, Random.Range(min_Angle, max_Angle), 0);

        
        nav.speed = 10f;

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 50f, LayerMask.GetMask("Wall")))
        {
            roarPoints = hit.collider.transform.position;
        }

        nav.SetDestination(roarPoints);

        nav.isStopped = false;
            
        //Vector3 dir = this.transform.position
        //}
    }

    bool IsReachedRoarDestination()
    {
        float dis = Vector3.Distance(this.transform.position, roarPoints);

        if (dis < roar_Deviation)
        {
            //needNewRocation = true;
            return true;
        }
        else return false; 
    }

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        es = GetComponent<Enemy_Seacher>();
        nav = GetComponent<NavMeshAgent>();

        if (patrolPoints == null) isPatrol = false;
        else isPatrol = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (nowDead)
            return;


        switch (state)
        {
            case STATE.IDLE:
                roarCount = 0;

                break;
            case STATE.IDLE_PATROL:
                roarCount = 0;

                break;
            case STATE.ROAR:
                roarTimer += Time.deltaTime;

                if (roarCount < 4 && roarTimer <= 5f)
                {
                    if (roarCount == 0)
                    {
                        SetRoarDestination();
                    }
                    else
                    {
                        if(IsReachedRoarDestination())
                        {
                            SetRoarDestination();
                        }
                    }
                }
                else
                {
                    if (isPatrol) state = STATE.IDLE_PATROL;
                    else state = STATE.IDLE;
                }

                break;
            case STATE.FIND:


                break;
            case STATE.ATTACK:


                break;

            case STATE.DEAD:
                break;
            default:
                break;
        }





    }
}
