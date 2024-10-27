using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour
{
    Gun gun;
    Transform player;
    public Animator animator;
    public bool canShot = false;
    public Enemy_Patrol_Node curnode;
    public Enemy_Patrol_Node backupNode;
    public Vector3 startpos;
    public Vector3 nextpos; // 패트롤용 변수
    public bool nullChecker = false;
    public float max_Angle = 205f;
    public float min_Angle = 35f;
    public float roamer_Deviation = 3f;
    public float fireChecker = 0f;
    
    

    public GameObject bloodEffect;

    AudioSource SFX;
    public AudioClip deadSound;

    public float raycastDistance;
    public float meeleTimer = 0;
    int[] rotationlist = new int[4] { -90, 90, 180, -180 };

    public GameObject range;

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

    int roamerCount = 0;
    Vector3 roamerPoints;

    Vector3 startPoint;

    public float rotationTimer = 0f;

    public float roamerTimer = 0f;
    public float follow_Spare_Time = 0f;

    public bool lostUser = false;
    public bool isPatrol;
    public bool isRotateEnemy = false;
    public bool nowDead = false;
    public bool needNewRocation = false;

    public bool coroutineChecker = false;

    public STATE state;

    Transform tr;

    public Enemy_Seacher es;
    public NavMeshAgent nav;

    public float bugCountDown = 0f;

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

            //this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(this.transform.position.x, rotationlist[Random.Range(0, 3)], this.transform.position.z), Time.deltaTime * 5f);

            this.transform.Rotate(0, rotationlist[Random.Range(0, 3)], 0);
            rotationTimer = 0;
        }
    }

    void RotationEnemy(Vector3 location) // 적 유닛의 회전과 관련된 함수
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
        //animator = GetComponentInChildren<Animator>();

    }
    void Start()
    {
        tr = GetComponent<Transform>();
        es = GetComponent<Enemy_Seacher>();
        nav = GetComponent<NavMeshAgent>();
        gun = GetComponentInChildren<Gun>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        SFX = GetComponent<AudioSource>();

        startPoint = this.transform.position;

        animator = GetComponentInChildren<Animator>();

        if (!isPatrol)
        {
            state = STATE.IDLE;
        }
        else
        {
            animator.SetBool("isPatrol", true);
            state = STATE.IDLE_PATROL;
        }
        startpos = this.transform.position;
        //nextpos = curnode.Get_Now_Node();

    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null) Debug.Log("NULL");

        Debug.Log(state);

        if (nowDead)
        {
            animator.SetBool("isDead", true);
            state = STATE.DEAD;
        }
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

        if (nav.velocity != Vector3.zero) bugCountDown = 0;
        // 해당 변수는 ROAMER 상태일 때 증가함.
        // 따라서, 이동속도가 ROAMER 상태일 때, 0이 되면은 bugCountDown은 증가 시작
        // 그게 아닐 땐, BugCountDown은 0으로 초기화


        /*********************  FSM *********************/


        switch (state)
        {
            /*********************  IDLE 상태 *********************/

            case STATE.IDLE:

                if (animator != null)
                {
                    animator.SetInteger("State", 0);
                }
                else
                {
                    Debug.LogError("Animator is null in Update!");
                }
                rotationTimer += Time.deltaTime;

                nav.SetDestination(startPoint);

                float dis = Vector3.Distance(this.transform.position, startPoint);

                if (dis > 1f)
                {
                    animator.SetInteger("State", 1);
                    RotationEnemy(startPoint);
                }
                else animator.SetInteger("State", 0);


                if (isRotateEnemy)
                    RotationIdle();

                break;




            /*********************  IDLE_PATROL 상태 *********************/

            case STATE.IDLE_PATROL:
                animator.SetInteger("State", 0);

                //PatrolEnemy(patrolPoints);
                nav.autoBraking = true;
                nav.speed = 5f;
                nav.velocity = nav.desiredVelocity;
                if (nav.destination != null) RotationEnemy(nav.destination);
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




            /*********************  ROAMER 상태 *********************/

            case STATE.ROAMER:
                animator.SetInteger("State", 1);

                if (nav.destination != null) RotationEnemy(nav.destination);

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
                        if (IsReachedroamerDestination())
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
                animator.SetInteger("State", 1);

                //if (gun.gunName == "Meele")
                //{
                //    raycastDistance = es.raycastDistance_;

                //    if (raycastDistance < 1.2f) state = STATE.ATTACK;
                //}

                if (!lostUser)
                {
                    if (canShot)
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
                    if (follow_Spare_Time <= 3.5f)
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

                // gun class를 선언하고, start 또는 awake 함수에서 getcomponentchild를 통해 컴포넌트를 초기화한다.
                // gun class에서 gunName을 받아온다.
                // swicth 문으로 gunName을 넣고 각 문자열에 맞춰 공격 조정
                // 만약 어떤 총도 아니라면, 밀리니 디폴트에 밀리 공격 넣기
                // 밀리 공격은 애니메이션 속도 진행 변수를 넣어 해당 애니메이션 변수 초에 맞춰 sphereall을 불러 유저가 맞았는지 확인.
                // 맞았으면 유저 사망.

                //this.transform.LookAt(player);

                //fireChecker += Time.deltaTime;
                //nav.isStopped = true;

                if (lostUser) state = STATE.FIND;

                switch (gun.gunName)
                {
                    case "SMG":  //SMG일 때
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
                        else
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }

                        //if(gun.Enemy_Fire(fireChecker)) fireChecker = 0;

                        break; // 됐다 신이 내린 선물이다 이건


                    case "Rifle":  // 라이플일 때

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }
                        break;


                    case "HandGun": // 권총일 때

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }
                        break;


                    case "Shotgun":  // 샷건일 때 (다른 총들과 달리 for문을 돌려 탄환을 여러개 발사)
                                     // 동일 간격의 발사가 더 좋은가?
                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_Shotgun_Fire());
                        }
                        break;


                    default: // 밀리 일 때
                             // 전부 삭제 후, 수정 예정. 애니메이션이 만들어지는 타이밍에 작성 예정.
                             // 그 상태에서, 적 유닛이 일정 범위에 들어오게되면 근접 무기 공격 애니메이션이 실행되고,
                             // 그 때, 적 유닛이 장착하고 있는 근접 무기의 collider 활성화,
                             // 적 유닛이 들고 있는 근접 무기의 스크립트에 OnTrrigerEnter 함수 작성.
                             // OnTrriger 함수 내에, 만약 Player가 존재한다면, 유저 사망 처리.
                             // 만약 유저가 공격을 회피해 미스가 난 경우,
                             // 다시 근접 무기의 collider 비활성화 후, 공격 범위 내 유저가 있는지 확인
                             // 만일, 유저가 범위에 벗어 났으면 Find 상태로 전환.
                             // 그게 아니라면, 다시 공격 진행.

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else
                        {
                            animator.SetInteger("State", 2);

                            StartCoroutine(MeeleAttack());
                        }
                        //StartCoroutine(MeeleAttack());

                        //RaycastHit[] hits;

                        //if (0.3f < meeleTimer && meeleTimer <= 1f)
                        //{
                        //    hits = Physics.SphereCastAll(transform.position, 1.5f, Vector3.forward, 1.5f);
                        //    foreach (RaycastHit hit in hits)
                        //    {
                        //        if (hit.collider.CompareTag("Player") && hit.collider != null)
                        //        {
                        //            Debug.Log("유저 사망");
                        //        }

                        //        else state = STATE.FIND;
                        //    }
                        //}                       


                        break;
                }

                break;




            /*********************  DEAD 상태 *********************/

            case STATE.DEAD:
                //animator.SetBool("isDead", true);

                //this.gameObject.transform.Rotate(90, 0, 0);
                Destroy(range.gameObject);
                AlertImDead();

                if (GetComponent<Rigidbody>() == null)
                {
                    gameObject.AddComponent<Rigidbody>();
                    GameObject blood = Instantiate(bloodEffect);
                    blood.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
                    blood.transform.rotation = Quaternion.identity;
                    //this.gameObject.GetComponent<Rigidbody>().mass = 3;
                    this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    this.gameObject.GetComponent<CapsuleCollider>().radius = 1.5f;
                    SFX.PlayOneShot(deadSound);
                    nav.enabled = false;
                }
                //this.GetComponent<Rigidbody>(). = true;

                //return;

                break;
            default:
                break;
        }
    }


    private IEnumerator MeeleAttack()
    {
        if(!coroutineChecker)
        {

            animator.SetInteger("State", 2);
            gun.MeeleSFX();

            coroutineChecker = true;
            // 애니메이션이 끝날 때까지 기다림 (임의의 대기 시간)
            yield return new WaitForSeconds(1.1f);
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, 3.2f, Vector3.up);

            foreach(RaycastHit hit in hits)
            {
                if(hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    Debug.Log("유저사망");
                }
            }

            coroutineChecker = false;
            
        }
        else
        {
            yield return null;
        }

    }

}
