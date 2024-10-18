using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour
{
    Gun                             gun;
    Transform                       player;
    public                          bool canShot = false;
    public                          Enemy_Patrol_Node curnode;
    public                          Enemy_Patrol_Node backupNode;
    public                          Vector3 startpos;
    public                          Vector3 nextpos; // ��Ʈ�ѿ� ����
    public                          bool nullChecker = false;
    public                          float max_Angle = 205f;
    public                          float min_Angle = 35f;
    public                          float roamer_Deviation = 3f;
    public                          float fireChecker = 0f;

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
    //public                          GameObject[] patrolPoints;

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
    void SetroamerDestination() // NavMesh ������ ���� �� �̵� �Լ� (ROAMER ���� �� ���)
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

    void AlertImDead() // �� ������ DEAD ������ ��, 10f �� �α� �ٸ� �� ���� ROAMER ���·� (DEAD�� ���Ǵ� �Լ�)
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

    public void SetRoamerWithResetRoamerCount() // ROAMER�� ���Ǵ� ���� ���� (DEAD ������ ��, �ٸ� �� ������ ���¸� ��ȯ�� �� ���)
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
    bool IsReachedroamerDestination() // �������� �����ߴ°�? (ROAMER �� ���)
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

    void RotationIdle() // �� ������ ���ڸ� ȸ���� ���õ� �Լ�. isRotateEnemy �� ��, IDLE ������ �� ���ڸ� ȸ��, �ƴ� �� ȸ�� X (IDLE�� ���) 
    {
        if (rotationTimer > 3f)
        {
            //this.gameObject.transform.rotation
            //    = Quaternion.Slerp(this.gameObject.transform.rotation,
            //                    Quaternion.Euler(this.transform.rotation.x, 
            //                                    this.gameObject.transform.rotation.y + rotationlist[Random.Range(0, 3)],
            //                                    this.gameObject.transform.rotation.z),
            //                    0.1f);

            //this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(this.transform.position.x, rotationlist[Random.Range(0, 3)], this.transform.position.z), Time.deltaTime * 5f);

            this.transform.Rotate(0, rotationlist[Random.Range(0, 3)], 0);
            rotationTimer = 0;
        }
    }

    void RotationEnemy(Vector3 location) // �� ������ ȸ���� ���õ� �Լ�
    {
        Vector3 dir = location - this.transform.position;
        dir.y = 0;


        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Start()
    {
        tr = GetComponent<Transform>();
        es = GetComponent<Enemy_Seacher>();
        nav = GetComponent<NavMeshAgent>();
        gun = GetComponentInChildren<Gun>();

        startPoint = this.transform.position;

        if (!isPatrol)
        {
            state = STATE.IDLE;
        }
        else
        {
            state = STATE.IDLE_PATROL;
        }
        startpos = this.transform.position;
        //nextpos = curnode.Get_Now_Node();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state);

        if (nowDead)
            state = STATE.DEAD;

        if (nav.enabled)
        {

            if (state != STATE.FIND)
                follow_Spare_Time = 0;

            if (state != STATE.IDLE_PATROL) nav.autoBraking = false;

            if (state != STATE.ATTACK) { fireChecker = 0; nav.isStopped = false; }

        }
        if (state != STATE.ROAMER)
        {
            roamerCount = 0;
            roamerTimer = 0;
        }

        if(nav.velocity != Vector3.zero) bugCountDown = 0;
        // �ش� ������ ROAMER ������ �� ������.
        // ����, �̵��ӵ��� ROAMER ������ ��, 0�� �Ǹ��� bugCountDown�� ���� ����
        // �װ� �ƴ� ��, BugCountDown�� 0���� �ʱ�ȭ


        /*********************  FSM *********************/


        switch (state)
        {
            /*********************  IDLE ���� *********************/

            case STATE.IDLE:
                rotationTimer += Time.deltaTime;

                nav.SetDestination(startPoint);

                if(isRotateEnemy)
                    RotationIdle();

                break;




            /*********************  IDLE_PATROL ���� *********************/

            case STATE.IDLE_PATROL:
                //PatrolEnemy(patrolPoints);
                nav.autoBraking = true;
                nav.speed = 5f;
                nav.velocity = nav.desiredVelocity;
                if(nav.destination != null) RotationEnemy(nav.destination);
                if (nullChecker)
                {
                    nav.SetDestination(startpos);
                    //Debug.Log(nav.destination);
                    if (Vector3.Distance(startpos, this.transform.position) < 1.5f)
                    {
                        Debug.Log(nav.destination);

                        curnode = backupNode;
                        nullChecker = false;
                    }
                }

                else
                {

                    if (curnode.next_Node == null)
                    {
                        nav.SetDestination(curnode.Get_Now_Node());

                        if (Vector3.Distance(curnode.Get_Now_Node(), this.transform.position) < 0.1f)
                        {
                            nullChecker = true;
                        }
                    }

                    else
                    {
                        nextpos = curnode.Get_Next_Node();

                        nav.SetDestination(nextpos);

                        if (Vector3.Distance(nextpos, this.transform.position) < 0.1f)
                        {
                            curnode = curnode.next_Node;
                            nextpos = curnode.Get_Now_Node();
                        }
                    }
                }
            

                break;




            /*********************  ROAMER ���� *********************/

            case STATE.ROAMER:
                if (nav.destination != null) RotationEnemy(nav.destination);

                roamerTimer += Time.deltaTime;
                bugCountDown += Time.deltaTime;
                if (bugCountDown > 1.5f)
                {
                    SetroamerDestination();
                }
                // �� �����̴� ���� ���� = �� ���ֳ��� ����ų�, ������Ʈ�� ����� Navmesh�� �������� �������� ����.
                // 1.5�ʰ� ���� ���ܼ� �������� ���� �� SetRoamerDestination()�Լ��� �ٽ� �ҷ� ������ �缳������ �ذ�

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





            /*********************  FIND ���� *********************/

            case STATE.FIND:
                if(!lostUser)
                {
                    if(canShot)
                    {
                        this.state = STATE.ATTACK;
                    }
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





            /*********************  ATTACT ���� *********************/

            case STATE.ATTACK:
                // gun class�� �����ϰ�, start �Ǵ� awake �Լ����� getcomponentchild�� ���� ������Ʈ�� �ʱ�ȭ�Ѵ�.
                // gun class���� gunName�� �޾ƿ´�.
                // swicth ������ gunName�� �ְ� �� ���ڿ��� ���� ���� ����
                // ���� � �ѵ� �ƴ϶��, �и��� ����Ʈ�� �и� ���� �ֱ�
                // �и� ������ �ִϸ��̼� �ӵ� ���� ������ �־� �ش� �ִϸ��̼� ���� �ʿ� ���� sphereall�� �ҷ� ������ �¾Ҵ��� Ȯ��.
                // �¾����� ���� ���.
                
                //this.transform.LookAt(player);

                //fireChecker += Time.deltaTime;
                nav.isStopped = true;

                switch (gun.gunName)
                {
                    case "SMG":  //SMG�� ��
                                 //if (!canShot)
                                 //{
                                 //    this.state = STATE.FIND;
                                 //    break;
                                 //}
                                 //else StartCoroutine(gun.Enemy_fire());

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else StartCoroutine(gun.Enemy_fire());

                        //if(gun.Enemy_Fire(fireChecker)) fireChecker = 0;

                        break; // �ƴ� ���� ���� �����̴� �̰�


                    case "Rifle":  // �������� ��

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else StartCoroutine(gun.Enemy_fire());

                        break;


                    case "HandGun": // ������ ��

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else StartCoroutine(gun.Enemy_fire());

                        break;


                    case "Shotgun":  // ������ �� (�ٸ� �ѵ�� �޸� for���� ���� źȯ�� ������ �߻�)
                                     // ���� ������ �߻簡 �� ������?
                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else StartCoroutine(gun.Enemy_Shotgun_Fire());

                        break;


                    default: // �и� �� ��


                        break;
                }

                break;




            /*********************  DEAD ���� *********************/

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
