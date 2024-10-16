using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour
{
    public                               float max_Angle = 205f;
    public                               float min_Angle = 35f;
    public                               float roamer_Deviation = 1.5f;
    [SerializeField]                     int[] rotationlist = new int[4] { -90, 90, 180, -180 };
    public                               GameObject range;
    public                               GameObject[] patrolPoints;
    [SerializeField]                     int roamerCount = 0;
    [SerializeField]                     Vector3 roamerPoints;
    [SerializeField]                     Vector3 startPoint;
    [SerializeField]                     Transform tr;
    public float rotationTimer = 0f;
    public                               float roamerTimer = 0f;
    public                               float follow_Spare_Time = 0f;
    public                               bool lostUser = false;
    public                               bool isPatrol;
    public                               bool isRotateEnemy = false;
    public                               bool nowDead = false;
    public                               bool needNewRocation = false;

    public                               STATE state;
    
    public                               Enemy_Seacher es;
    public                               NavMeshAgent nav;

    public enum STATE
    {
        IDLE = 0,
        IDLE_PATROL,
        ROAMER,
        FIND,
        ATTACK,
        DEAD
    }

    public void SetStateFInd()
    {
        state = STATE.FIND;
    }
    void SetroamerDestination()
    {
        //if(roamerCount < 4)
        //{
        roamerCount++;

        nav.isStopped = true;

        transform.Rotate(0, Random.Range(min_Angle, max_Angle), 0);

        
        nav.speed = 7f;

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 50f, LayerMask.GetMask("Wall")))
        {
            roamerPoints = hit.collider.transform.position;
        }

        nav.SetDestination(roamerPoints);

        nav.isStopped = false;
            
        //Vector3 dir = this.transform.position
        //}
    }

    /*
    bool IsReachedPatrolDestination(Transform t)
    {
        float dis = Vector3.Distance(this.transform.position, t.position);

        if(dis < 0.5f)
        {
            return true;
        }

        return false;
    }*/

    /*
    void PatrolEnemy(GameObject[] location)
    {
        foreach (GameObject t in location)
        {
            nav.speed = 5f;
            nav.velocity = nav.desiredVelocity;

            nav.SetDestination(t.transform.position);

            Debug.Log(t.name);
            
            while(IsReachedPatrolDestination(t.transform))
            {
                //nav.SetDestination(t.transform.position);
            }
        }

    }*/

    void AlertImDead()
    {
        RaycastHit[] hits;

        hits = Physics.SphereCastAll(transform.position, 10f, Vector3.up);
        foreach (RaycastHit hit in hits) {
            if (hit.collider != null 
                && hit.collider.tag == "Enemy" 
                && hit.collider.gameObject != this.gameObject
                && 
                    (
                    hit.collider.GetComponent<FSM>().state == STATE.IDLE 
                    || hit.collider.GetComponent<FSM>().state == STATE.IDLE_PATROL 
                    || hit.collider.GetComponent<FSM>().state == STATE.ROAMER
                    )                
                )
            {
                hit.collider.GetComponent<FSM>().SetRoamerWithResetRoamerCount();
                Debug.Log(hit.collider.tag);
            }
        }
    }

    public void SetRoamerWithResetRoamerCount()
    {
        if (this.state != STATE.ROAMER)
            this.state = STATE.ROAMER;
        else
        {
            if (this.roamerCount >= 10)
                this.roamerCount = 0;

            if (this.roamerTimer >= 8f)
                this.roamerTimer = 0f;
        }
    }
    bool IsReachedroamerDestination()
    {
        float dis = Vector3.Distance(this.transform.position, roamerPoints);

        if (dis < roamer_Deviation)
        {
            nav.velocity = nav.desiredVelocity;
            //needNewRocation = true;
            return true;
        }
        else return false; 
    }

    public void SetStateDead()
    {
        nowDead = true;
    }

    void RotationIdle()
    {
        if (rotationTimer > 3f)
        {
            //this.gameObject.transform.rotation
            //    = Quaternion.Slerp(this.gameObject.transform.rotation,
            //                    Quaternion.Euler(this.transform.rotation.x, 
            //                                    this.gameObject.transform.rotation.y + rotationlist[Random.Range(0, 3)],
            //                                    this.gameObject.transform.rotation.z),
            //                    0.1f);

            this.transform.Rotate(0, rotationlist[Random.Range(0, 3)], 0);
            rotationTimer = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        es = GetComponent<Enemy_Seacher>();
        nav = GetComponent<NavMeshAgent>();
        
        startPoint = this.transform.position;

        if (!isPatrol)
        {
            state = STATE.IDLE;
        }
        else
        {
            state = STATE.IDLE_PATROL;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (nowDead)
            state = STATE.DEAD;

        if (state != STATE.FIND)
            follow_Spare_Time = 0;

        if (state != STATE.ROAMER)
        {
            roamerCount = 0;
            roamerTimer = 0;
        }


        switch (state)
        {
            case STATE.IDLE:
                rotationTimer += Time.deltaTime;

                nav.SetDestination(startPoint);

                if(isRotateEnemy)
                    RotationIdle();

                break;
            case STATE.IDLE_PATROL:
                //PatrolEnemy(patrolPoints);

                break;
            case STATE.ROAMER:
                roamerTimer += Time.deltaTime;

                if (roamerCount < 10 && roamerTimer <= 8f)
                {
                    if (roamerCount == 0)
                    {
                        SetroamerDestination();
                    }
                    else
                    {
                        if(IsReachedroamerDestination())
                        {
                            SetroamerDestination();
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
                if(!lostUser)
                {
                    follow_Spare_Time = 0;

                    es.RotateToUser();
                    es.FollowUser();
                }
                else
                {
                    follow_Spare_Time += Time.deltaTime;
                    if(follow_Spare_Time <= 3.5f)
                    {
                        es.RotateToUser();
                        es.FollowUser();
                    }
                    else
                    {
                        state = STATE.ROAMER;
                    }
                }

                break;
            case STATE.ATTACK:


                break;

            case STATE.DEAD:
                //this.gameObject.transform.Rotate(90, 0, 0);
                Destroy(range.gameObject);
                AlertImDead();

                if (GetComponent<Rigidbody>() == null)
                {
                    gameObject.AddComponent<Rigidbody>();
                    this.gameObject.GetComponent<Rigidbody>().mass = 3;
                    nav.enabled = false;
                }
                    //this.GetComponent<Rigidbody>(). = true;

                //return;

                break;
            default:
                break;
        }





    }
}
