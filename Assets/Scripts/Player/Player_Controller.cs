using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.MeshOperations;

public class Player_Controller : MonoBehaviour
{

    public                                      float speed = 5f;
    public                                      Transform shotRocation;
    public                                      Gun gun;

    public float margin_of_error = 0.3f;

    public bool isMelee = true;
    public                                      bool possible_Kill_Confirm = false;

    public                                      bool nowReroadingRunAnimation = false;

    public int dirpos = 0;
    [SerializeField]                            float timechecker = 0;
    [SerializeField]                            Player_LookAtController childLC;
    [SerializeField]                            Animator animator;

    //private Enemy_Seacher[] es;

    //FSM[] fsm;

    // Start is called before the first frame update
    void Start()
    {
        //es = GameObject.FindGameObjectsWithTag("Enemy").Getcomponent<
        childLC = GetComponentInChildren<Player_LookAtController>();
        animator = GetComponentInChildren<Animator>();

        /*
               현재 필요 로직
        1.
               Gun을 start에서 GetComponentInChildren으로 받는다.
                Gun이 null이면, 주먹 밀리 공격 이벤트 실행
                Gun이 Meele면 빠따 공격 이벤트 실행(셋불 has_meele_item = true)
                Gun이 총기면, 각 총기별 공격 이벤트  실행.
                총의 종류에 따라, 애니메이터의 파라메터 bool 값을 수정.

        2.     
                각 무기별, 레이케스트 사격.
                근접 공격과 샷건을 제외한 총기 사격은 레이케스트로 이뤄진 사격 함수를 작성해 진행한다.
                각 총기별 사거리에 제한을 두도록한다.

        3.      
                줍총
                적이 죽으면, 적이 가지고 있는 총기를 떨군다.
                떨군 총기는, 시체 위로 Ins 하고, 유저는 이를 G키를 눌러 먹는다.
                만일, 총기를 가지고 있다면, 이 무기와 교체한다.
                교체했을 때, 계획서에 작성한 로직대로 총기의 탄과 잔탄을 부여 받는다.       

        4.
                사격에 용이하기 위해, 현재 마우스 포인트가 위치한 곳에 크로스헤어 그림을 위치한다.

        5.
                무기를 사격할 때, 머즐이펙트와 사운드를 출력하도록 한다.

        6.
                무기 투척
                플레이어가 마우스 우클릭으로 했을 때, 크로스헤어의 디자인이 바뀌며, 그 상태에서 좌클릭을 눌렀을 때, 해당 무기에 RigidBody가 달리고, 중력 제한은 fasle로, Addforce를 사용하여 직선 방향으로 총기가 날라가도록 한다.
                해당 방향으로 날라간 무기가, 적에게 닿으면 적 사망. 콜라이더에 충돌 판정이 들어오면, 중력 제한을 true로 놓고 무기가 땅으로 떨어지도록 한다.
                땅에 떨어진 무기는 줍총과 관련된 함수를 사용하여 자신과 동일한(탄, 잔탄 등) 무기를 ins하고 distroy 한다.

        7.      
                사운드 조절이 가능한 BGM을 넣는다.
         
        8.      
                카메라의 이동
                카메라의 이동은 계획서에 있는 대로 진행한다. 허나, 해당 로직은 게임 플레이에 중요치 않은 요소이기에, 마지막에 구현하도록 한다.
        
         */
    }

    // Update is called once per frame
    void Update()
    {

        timechecker += Time.deltaTime;


        PlayerMove();
        childLC.PlayerRotate();//PlayerRotate(childTR);
        Debug.DrawRay(shotRocation.position, shotRocation.forward, Color.red);

        if (Input.GetKeyDown(KeyCode.R))
            ReRoadingInvoke(gun.reRoadTime);

        if(gun.Fire(timechecker))
        {
            AlertToEnemy();
            timechecker = 0;
            Debug.Log("사격");            
        }
         

        // 현재 테스트용. 추후 총기의 연사력을 기준으로 GetKey 상태일 때 총알이 레이케스트로 나가야 함.
    }

    void Kill_Confirm(GameObject go) // 확인 사살과 관련된 함수. 추후 애니메이션 변수가 들어가야 함.
    {
        Destroy(go.gameObject);
    }

    /*
    public void ShotBullet()
    {
        RaycastHit hit;

        if (Physics.Raycast(shotRocation.position, shotRocation.forward, out hit, LayerMask.GetMask("Enemy", "Wall")))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("적 상태 Dead");
            }
        }
    }*/

    IEnumerator ReRoadingInvoke(float reroadingTime) // 재장전 IEnumerator
    {
        gun.now_Reroading = true;
        nowReroadingRunAnimation = true;
        yield return new WaitForSeconds(reroadingTime);

        gun.ReRoad();
        gun.now_Reroading = false;
        nowReroadingRunAnimation = false;
    }

    public void OnTriggerStay(Collider other) // 적 유닛의 확인 사살을 위해 사용되는 Trigger 함수. 
    {
        if(!other.gameObject.CompareTag("Player"))
            Debug.Log(other.tag);

        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<FSM>().state == FSM.STATE.DEAD)
        {
            //Destroy(other.gameObject);
            if(Input.GetKeyDown(KeyCode.E))
                Kill_Confirm(other.gameObject);
        }
    } // 현재 키가 씹히는 문제가 있음. <- 해결

    void PlayerMove() // 플레이어 캐릭터의 ㄴ이동과 관련된 함수 
    {
        float transform_z = Input.GetAxis("Vertical") * speed;
        float transform_x = Input.GetAxis("Horizontal") * speed;

        transform_z *= Time.deltaTime;
        transform_x *= Time.deltaTime;


        /*Vector3 playerMoveDir = new Vector3(transform_x, 0, transform_z);
        Vector3 nomalDir = new Vector3(playerMoveDir.normalized.x , 0, playerMoveDir.normalized.z);

        Vector3 getRotationVector = childLC.ReturnNormalDir();

        float angle = Vector3.Angle(getRotationVector, nomalDir);

        Debug.Log(angle);
        if (transform_z == 0 && transform_x == 0)
        {
            dirpos = 5;
        }
        else
        {

            if (nomalDir.x + margin_of_error < getRotationVector.x) // 캐릭터의 x 이동이 왼쪽 (7, 4, 1)
            {
                if (nomalDir.z + margin_of_error < getRotationVector.z) // 캐릭터의 z 이동이 뒤 (1, 2, 3)
                {
                    dirpos = 1;
                }
                else if (nomalDir.z + margin_of_error == 0) // 캐릭터의 z 이동이 없음 (4, 5, 6)
                {
                    dirpos = 4;
                }
                else // 캐릭터의 z 이동이 앞 (7, 8, 9)
                {
                    dirpos = 7;
                }
            }
            else if (nomalDir.x + margin_of_error == 0) // 캐릭터의 x 이동 없음 (8, 5, 2)
            {
                if (nomalDir.z + margin_of_error < getRotationVector.z)
                {
                    dirpos = 2;
                }
                //else if (nomalDir.z + margin_of_error == 0)
                //{
                //    dirpos = 5;
                //}
                else
                {
                    dirpos = 8;
                }
            }
            else // 캐릭터의 x 이동이 오른쪽 (9, 6, 3)
            {
                if (nomalDir.z + margin_of_error < getRotationVector.z)
                {
                    dirpos = 3;
                }
                else if (nomalDir.z + margin_of_error == 0)
                {
                    dirpos = 6;
                }
                else
                {
                    dirpos = 9;
                }
            }
        } // 1안*/

        Vector3 playerMoveDir = transform.TransformDirection(new Vector3(transform_x, 0, transform_z)).normalized;

        //Vector3 playerMoveDir = new Vector3(transform_x, 0, transform_z); // 플레이어 이동 방향
        Vector3 getRotationVector = childLC.ReturnDir(); // 캐릭터의 현재 바라보는 방향 벡터


        // 이동이 있을 때만 각도를 계산
        if (playerMoveDir != Vector3.zero)
        {
            float angle = Vector3.SignedAngle(getRotationVector, playerMoveDir, Vector3.up);

            // 디버깅을 위한 로그
            Debug.Log($"Move Direction: {playerMoveDir}, Rotation: {getRotationVector}, Angle: {angle}");

            // 각도를 양수로 변환 (0~360도)
            if (angle < 0)
                angle += 360;

            // 8방향 판정
            if (angle >= 337.5f || angle < 22.5f)
            {
                dirpos = 8; // 정면
            }
            else if (angle >= 22.5f && angle < 67.5f)
            {
                dirpos = 9; // 우측 대각선 앞
            }
            else if (angle >= 67.5f && angle < 112.5f)
            {
                dirpos = 6; // 오른쪽
            }
            else if (angle >= 112.5f && angle < 157.5f)
            {
                dirpos = 3; // 우측 대각선 뒤
            }
            else if (angle >= 157.5f && angle < 202.5f)
            {
                dirpos = 2; // 뒤쪽
            }
            else if (angle >= 202.5f && angle < 247.5f)
            {
                dirpos = 1; // 좌측 대각선 뒤
            }
            else if (angle >= 247.5f && angle < 292.5f)
            {
                dirpos = 4; // 왼쪽
            }
            else if (angle >= 292.5f && angle < 337.5f)
            {
                dirpos = 7; // 좌측 대각선 앞
            }

            // 각도를 계산해서 360도 기준으로 방향을 나눈다.
            /*float angle = Vector3.SignedAngle(getRotationVector, playerMoveDir, Vector3.up);

            Debug.DrawRay(transform.position, playerMoveDir * 2, Color.red);
            Debug.DrawRay(transform.position, getRotationVector * 2, Color.blue);

            //Debug.Log(angle);

            // 각도에 따라 8방향으로 나눈다
            if (angle >= -22.5f && angle < 22.5f)
            {
                dirpos = 8; // 정면 (앞)
            }
            else if (angle >= 22.5f && angle < 67.5f)
            {
                dirpos = 9; // 우측 대각선 앞
            }
            else if (angle >= 67.5f && angle < 112.5f)
            {
                dirpos = 6; // 오른쪽
            }
            else if (angle >= 112.5f && angle < 157.5f)
            {
                dirpos = 3; // 우측 대각선 뒤
            }
            else if ((angle >= 157.5f && angle <= 180f) || (angle < -157.5f && angle >= -180f))
            {
                dirpos = 2; // 뒤쪽
            }
            else if (angle >= -157.5f && angle < -112.5f)
            {
                dirpos = 1; // 좌측 대각선 뒤
            }
            else if (angle >= -112.5f && angle < -67.5f)
            {
                dirpos = 4; // 왼쪽
            }
            else if (angle >= -67.5f && angle < -22.5f)
            {
                dirpos = 7; // 좌측 대각선 앞
            } // 2-1안 */
        }
        else
        {
            dirpos = 5; // 이동하지 않을 때 (정지 상태)
        } // 2안 회전각 사용*/


        animator.SetInteger("MovePos", dirpos);

        // 이동 적용
        this.gameObject.transform.Translate(transform_x, 0, transform_z);
    }

    /*
    public void PlayerRotate() // 캐릭터의 이동과 회전을 부모 자식과 분해해서 사용. 겹쳐 사용하니 조작감이 이상함.
        //스크립트 분리가 필요.
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
        Plane site = new Plane(Vector3.up, Vector3.up);

        if(site.Raycast(ray, out float dis))
        {
            Vector3 dir = ray.GetPoint(dis) - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        }
    }
    */

    void AlertToEnemy() // 총을 쏠 때, 20f 범위 내, 적에게 유저의 위치를 알리는 함수.
    {
        //총을 쏠 때, 테스트용
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, 20f, Vector3.up);
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<FSM>().SetStateFInd();                
                Debug.Log("적에게 알림");
            }
        }

        // 유저가 사격했을 시, 해당 함수 실행
    }
}
