using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    Player_LookAtController childLC;

    public float speed = 5f;
    public Transform shotRocation;
    public Gun gun;

    public bool isMelee = true;
    float timechecker = 0;

    public bool possible_Kill_Confirm = false;

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
            Debug.Log("���");            
        }


        PlayerMove();
        childLC.PlayerRotate();//PlayerRotate(childTR);
        Debug.DrawRay(shotRocation.position, shotRocation.forward, Color.red);

        // ���� �׽�Ʈ��. ���� �ѱ��� ������� �������� GetKey ������ �� �Ѿ��� �����ɽ�Ʈ�� ������ ��.
    }

    void Kill_Confirm(GameObject go)
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
                Debug.Log("�� ���� Dead");
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

    public void OnTriggerStay(Collider other)
    {
        if(!other.gameObject.CompareTag("Player"))
            Debug.Log(other.tag);

        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<FSM>().state == FSM.STATE.DEAD)
        {
            //Destroy(other.gameObject);
            if(Input.GetKeyDown(KeyCode.E))
                Kill_Confirm(other.gameObject);
        }
    } // ���� �����۵� ���� ����.

    void PlayerMove()
    {
        float transform_z = Input.GetAxis("Vertical") * speed;
        float transform_x = Input.GetAxis("Horizontal") * speed;

        transform_z *= Time.deltaTime;
        transform_x *= Time.deltaTime;

        this.gameObject.transform.Translate(transform_x, 0, transform_z);
    }

    /*
    public void PlayerRotate() // ĳ������ �̵��� ȸ���� �θ� �ڽİ� �����ؼ� ���. ���� ����ϴ� ���۰��� �̻���.
        //��ũ��Ʈ �и��� �ʿ�.
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

    void AlertToEnemy()
    {
        //���� �� ��, �׽�Ʈ��
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, 20f, Vector3.up);
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<FSM>().SetStateFInd();                
                Debug.Log("������ �˸�");
            }
        }

        // ������ ������� ��, �ش� �Լ� ����
    }
}
