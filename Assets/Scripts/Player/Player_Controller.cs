using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    public                                      float speed = 5f;
    public                                      Transform shotRocation;
    public                                      Gun gun;

    public                                      bool isMelee = true;
    public                                      bool possible_Kill_Confirm = false;

    public                                      bool nowReroadingRunAnimation = false;

    [SerializeField]                            float timechecker = 0;
    [SerializeField]                            Player_LookAtController childLC;

    //private Enemy_Seacher[] es;

    //FSM[] fsm;

    // Start is called before the first frame update
    void Start()
    {
        //es = GameObject.FindGameObjectsWithTag("Enemy").Getcomponent<
        childLC = GetComponentInChildren<Player_LookAtController>();
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
