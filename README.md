SOS
-------------------------
제작 기간 1달 : (24-10-07 ~ 24-11-04)
탑 다운 슈팅 게임 개인 프로젝트입니다.

---------

개발환경
---------
Unity, C#

---------

플레이어블 플랫폼
---------
Windows

---------

플레이어 목표
---------
[최소 목표]적을 사살하고 클리어 포인트로 향할 것.

[확장 목표]최대한 고득점을 하여 게임을 클리어할 것.

---------

조작키
--------
|입력키|설명|
|------|---|
|W|플레이어 캐릭터의 전진 이동|
|S|플레이어 캐릭터의 후진 이동|
|A|플레이어 캐릭터의 좌측 이동|
|D|플레이어 캐릭터의 우측 이동|
|Mouse Location|해당 방향으로 캐릭터 회전 및 사격 방향 조절|
|Mouse Button 0|사격 또는 근접 공격|
|E|적 시체에 유저의 캐릭터가 가까이 있을 경우, 확인 사살|
|G|현재 무기 버리기, 또는 무기에 가까이 있을 경우 해당 무기로 교체|

--------

적의 상태머신
--------

|상태명|설명|
|------|---|
|IDLE|기본적인 상태. 적 유닛의 자식 오브젝트로 달린 Find 오브젝트의 Trigger을 통해, 유저를 찾음. 찾았을 경우 FIND 상태로 전환|
|IDLE_Patrol|IDLE 상태이나, 순찰하는 유닛. 노드를 통해 구현하였음.|
|Find|적 유닛이 플레이어를 발견하였고, 추격하는 상태. 이때, 같은 적 유닛의 자식 오브젝트로 달린 Attack 오브젝트의 콜라이더가 Trigger 되었을 때, Attack으로 전환|
|ROAMER|적 유닛이 Find 상태에서 유저를 놓쳤고, 주워진 유예 시간 내에서도 유저를 찾지 못한 경우, 또는 근처에 적 유닛의 시체가 존재하고 있을 경우 전환되는 상태. 랜덤한 방향으로 회전하며 일정 시간 동안 흥분 상태에 빠짐.|
|Attack|플레이어를 향해 공격 진행|

상태머신 진행도

                        IDLE (OR IDLE_PATROL)      ㅡㅡ             ROAMER
                                  |                 X                 |
                                FIND               ㅡㅡ             ATTACK

FSM Switch문
-------
```
        switch (state)
        {
            /*********************  IDLE *********************/

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




            /*********************  IDLE_PATROL *********************/

            case STATE.IDLE_PATROL:
                animator.SetInteger("State", 0);
                if (!isPatrol)
                {
                    state = STATE.IDLE;
                }
                else
                {
                    animator.SetBool("isPatrol", true);
                    state = STATE.IDLE_PATROL;
                }
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

                        if (Vector3.Distance(curnode.Get_Now_Node(), this.transform.position) < 0.3f)
                        {
                            nullChecker = true;
                        }
                    }

                    else
                    {
                        nextpos = curnode.Get_Next_Node();

                        nav.SetDestination(nextpos);

                        if (Vector3.Distance(nextpos, this.transform.position) < 0.3f)
                        {
                            curnode = curnode.next_Node;
                            nextpos = curnode.Get_Now_Node();
                        }
                    }
                }


                break;




            /*********************  ROAMER *********************/

            case STATE.ROAMER:
                animator.SetInteger("State", 1);

                if (nav.destination != null) RotationEnemy(nav.destination);

                roamerTimer += Time.deltaTime;
                bugCountDown += Time.deltaTime;
                if (bugCountDown > 1.5f)
                {
                    SetroamerDestination();
                }

                // ROAMER 상태일 때, 도착을 하지 못했을 경우 실행.
                // 1.5초가 넘게 되면 새로운 경로를 찾도록 SetRoamerDestination() 함수를 호출함.

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



            /*********************  FIND *********************/

            case STATE.FIND:
                animator.SetInteger("State", 1);

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





            /*********************  ATTACT  *********************/

            case STATE.ATTACK:

                // gun class를 상속하고, start 또는 awake 함수에서 getcomponentchild로 컴포넌트를 초기화한다.
                // gun class에서 gunName을 가져온다.
                // switch 문에서 gunName을 사용해 무기를 설정.


                //this.transform.LookAt(player);

                //fireChecker += Time.deltaTime;
                //nav.isStopped = true;

                if (lostUser) state = STATE.FIND;

                switch (gun.gunName)
                {
                    case "SMG":  //SMG

                        if (!canShot)
                        {

                            this.state = STATE.FIND;
                            break;
                        }
                        else if(es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }

                        //if(gun.Enemy_Fire(fireChecker)) fireChecker = 0;

                        break; 


                    case "Rifle":  // �������� ��

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }
                        break;


                    case "HandGun":

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_fire());
                        }
                        break;


                    case "Shotgun": 

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);
                            RotationEnemy(player.transform.position);
                            StartCoroutine(gun.Enemy_Shotgun_Fire());
                        }
                        break;


                    default: 

                        if (!canShot)
                        {
                            this.state = STATE.FIND;
                            break;
                        }
                        else if (es.SearchUser())
                        {
                            animator.SetInteger("State", 2);

                            StartCoroutine(MeeleAttack());
                        }                     


                        break;
                }

                break;




            /*********************  DEAD  *********************/

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

                    DropGun();
                    //this.gameObject.GetComponent<Rigidbody>().mass = 3;
                    this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    this.gameObject.GetComponent<CapsuleCollider>().radius = 1.5f;
                    SFX.PlayOneShot(deadSound);
                    nav.enabled = false;

                    clear.RemoveListEnemy(this.gameObject);
                }
                //this.GetComponent<Rigidbody>(). = true;

                //return;

                break;
            default:
                break;
        }
```

---------

적의 추격 조건
--------


