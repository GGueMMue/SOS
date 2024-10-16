using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour
{
    public                          float max_Angle = 205f;
    public                          float min_Angle = 35f;
    public                          float roamer_Deviation = 3f;

    int[] rotationlist =            new int[4] { -90, 90, 180, -180 };

    public                          GameObject range;

    public enum STATE
    {
        IDLE = 0,
        IDLE_PATROL,
        ROAMER,
        FIND,
        ATTACK,
        DEAD
    }
    public                          GameObject[] patrolPoints;

    int                             roamerCount = 0;
                                    Vector3 roamerPoints;

                                    Vector3 startPoint;

    public                          float rotationTimer = 0f;

    public                          float roamerTimer = 0f;
    public                          float follow_Spare_Time = 0f;

    public                          bool lostUser = false;
    public                          bool isPatrol;
    public                          bool isRotateEnemy = false;
    public                          bool nowDead = false;
    public                          bool needNewRocation = false;

    public                          STATE state;

                                    Transform tr;

    public                          Enemy_Seacher es;
    public                          NavMeshAgent nav;

    public                          float bugCountDown = 0f;

    public void SetStateFInd()
    {
        state = STATE.FIND;
    }
    void SetroamerDestination() // NavMesh 목적지 생성 및 이동 함수 (ROAMER 상태 시 사용)
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

    void AlertImDead() // 적 유닛이 DEAD 상태일 때, 10f 내 인근 다른 적 유닛 ROAMER 상태로 (DEAD시 사용되는 함수)
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

    public void SetRoamerWithResetRoamerCount() // ROAMER에 사용되는 변수 리셋 (DEAD 상태일 때, 다른 적 유닛의 상태를 변환할 때 사용)
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
    bool IsReachedroamerDestination() // 목적지에 도착했는가? (ROAMER 시 사용)
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

    void RotationIdle() // 적 유닛의 제자리 회전과 관련된 함수. isRotateEnemy 일 땐, IDLE 상태일 때 제자리 회전, 아닐 땐 회전 X (IDLE시 사용) 
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

        if(nav.velocity != Vector3.zero) bugCountDown = 0;
        // 해당 변수는 ROAMER 상태일 때 증가함.
        // 따라서, 이동속도가 ROAMER 상태일 때, 0이 되면은 bugCountDown은 증가 시작
        // 그게 아닐 땐, BugCountDown은 0으로 초기화


        /*********************  FSM *********************/


        switch (state)
        {
            /*********************  IDLE 상태 *********************/

            case STATE.IDLE:
                rotationTimer += Time.deltaTime;

                nav.SetDestination(startPoint);

                if(isRotateEnemy)
                    RotationIdle();

                break;




            /*********************  IDLE_PATROL 상태 *********************/

            case STATE.IDLE_PATROL:
                //PatrolEnemy(patrolPoints);

                break;




            /*********************  ROAMER 상태 *********************/

            case STATE.ROAMER:
                roamerTimer += Time.deltaTime;
                bugCountDown += Time.deltaTime;
                if (bugCountDown > 1.5f)
                {
                    SetroamerDestination();
                }
                // 안 움직이는 버그 원인 = 적 유닛끼리 낑기거나, 오브젝트에 낑기면 Navmesh의 목적지에 도달하지 못함.
                // 1.5초간 서로 낑겨서 움직이지 못할 시 SetRoamerDestination()함수를 다시 불러 목적지 재설정으로 해결

                if (roamerCount < 10 || roamerTimer <= 8f)
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





            /*********************  FIND 상태 *********************/

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





            /*********************  ATTACT 상태 *********************/

            case STATE.ATTACK:


                break;




            /*********************  DEAD 상태 *********************/

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
