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
    public Vector3 nextpos; // ��Ʈ�ѿ� ����
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
        // �ش� ������ ROAMER ������ �� ������.
        // ����, �̵��ӵ��� ROAMER ������ ��, 0�� �Ǹ��� bugCountDown�� ���� ����
        // �װ� �ƴ� ��, BugCountDown�� 0���� �ʱ�ȭ


        /*********************  FSM *********************/


        switch (state)
        {
            /*********************  IDLE ���� *********************/

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




            /*********************  IDLE_PATROL ���� *********************/

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




            /*********************  ROAMER ���� *********************/

            case STATE.ROAMER:
                animator.SetInteger("State", 1);

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





            /*********************  FIND ���� *********************/

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
                //nav.isStopped = true;

                if (lostUser) state = STATE.FIND;

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
                        else
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }

                        //if(gun.Enemy_Fire(fireChecker)) fireChecker = 0;

                        break; // �ƴ� ���� ���� �����̴� �̰�


                    case "Rifle":  // �������� ��

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


                    case "HandGun": // ������ ��

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


                    case "Shotgun":  // ������ �� (�ٸ� �ѵ�� �޸� for���� ���� źȯ�� ������ �߻�)
                                     // ���� ������ �߻簡 �� ������?
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


                    default: // �и� �� ��
                             // ���� ���� ��, ���� ����. �ִϸ��̼��� ��������� Ÿ�ֿ̹� �ۼ� ����.
                             // �� ���¿���, �� ������ ���� ������ �����ԵǸ� ���� ���� ���� �ִϸ��̼��� ����ǰ�,
                             // �� ��, �� ������ �����ϰ� �ִ� ���� ������ collider Ȱ��ȭ,
                             // �� ������ ��� �ִ� ���� ������ ��ũ��Ʈ�� OnTrrigerEnter �Լ� �ۼ�.
                             // OnTrriger �Լ� ����, ���� Player�� �����Ѵٸ�, ���� ��� ó��.
                             // ���� ������ ������ ȸ���� �̽��� �� ���,
                             // �ٽ� ���� ������ collider ��Ȱ��ȭ ��, ���� ���� �� ������ �ִ��� Ȯ��
                             // ����, ������ ������ ���� ������ Find ���·� ��ȯ.
                             // �װ� �ƴ϶��, �ٽ� ���� ����.

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
                        //            Debug.Log("���� ���");
                        //        }

                        //        else state = STATE.FIND;
                        //    }
                        //}                       


                        break;
                }

                break;




            /*********************  DEAD ���� *********************/

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
            // �ִϸ��̼��� ���� ������ ��ٸ� (������ ��� �ð�)
            yield return new WaitForSeconds(1.1f);
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, 3.2f, Vector3.up);

            foreach(RaycastHit hit in hits)
            {
                if(hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    Debug.Log("�������");
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
