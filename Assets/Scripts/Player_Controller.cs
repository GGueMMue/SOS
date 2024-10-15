using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    public float speed = 5f;
    Player_LookAtController childLC;
    public Transform shotRocation;
    public Gun gun;

    public bool isMelee = true;
    float timechecker = 0;

    public bool nowReroadingRunAnimation = false;
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
        if (Input.GetKeyDown(KeyCode.R))
            ReRoadingInvoke(gun.reRoadTime);

        if(gun.Fire(timechecker))
        {
            AlertToEnemy();
            timechecker = 0;
            Debug.Log("사격");
            
        }

        PlayerMove();
        childLC.PlayerRotate();//PlayerRotate(childTR);
        Debug.DrawRay(shotRocation.position, shotRocation.forward, Color.red);

        // 현재 테스트용. 추후 총기의 연사력을 기준으로 GetKey 상태일 때 총알이 레이케스트로 나가야 함.
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

    IEnumerator ReRoadingInvoke(float reroadingTime)
    {
        gun.now_Reroading = true;
        nowReroadingRunAnimation = true;
        yield return new WaitForSeconds(reroadingTime);

        gun.ReRoad();
        gun.now_Reroading = false;
        nowReroadingRunAnimation = false;
    }

    public void PlayerMove()
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

    public void AlertToEnemy()
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
