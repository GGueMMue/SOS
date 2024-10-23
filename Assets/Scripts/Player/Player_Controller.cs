using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Player_Controller : MonoBehaviour
{

    public                                      float speed = 5f;
    public                                      Transform shotRocation;
    public                                      Gun gun;

    public float margin_of_error = 0.15f;

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
            Debug.Log("���");            
        }
         

        // ���� �׽�Ʈ��. ���� �ѱ��� ������� �������� GetKey ������ �� �Ѿ��� �����ɽ�Ʈ�� ������ ��.
    }

    void Kill_Confirm(GameObject go) // Ȯ�� ���� ���õ� �Լ�. ���� �ִϸ��̼� ������ ���� ��.
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

    IEnumerator ReRoadingInvoke(float reroadingTime) // ������ IEnumerator
    {
        gun.now_Reroading = true;
        nowReroadingRunAnimation = true;
        yield return new WaitForSeconds(reroadingTime);

        gun.ReRoad();
        gun.now_Reroading = false;
        nowReroadingRunAnimation = false;
    }

    public void OnTriggerStay(Collider other) // �� ������ Ȯ�� ����� ���� ���Ǵ� Trigger �Լ�. 
    {
        if(!other.gameObject.CompareTag("Player"))
            Debug.Log(other.tag);

        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<FSM>().state == FSM.STATE.DEAD)
        {
            //Destroy(other.gameObject);
            if(Input.GetKeyDown(KeyCode.E))
                Kill_Confirm(other.gameObject);
        }
    } // ���� Ű�� ������ ������ ����. <- �ذ�

    void PlayerMove() // �÷��̾� ĳ������ ���̵��� ���õ� �Լ� 
    {
        float transform_z = Input.GetAxis("Vertical") * speed;
        float transform_x = Input.GetAxis("Horizontal") * speed;

        transform_z *= Time.deltaTime;
        transform_x *= Time.deltaTime;

        Vector3 playerMoveDir = new Vector3(transform_x, 0, transform_z);
        Vector3 nomalDir = new Vector3(playerMoveDir.normalized.x , 0, playerMoveDir.normalized.z);

        Vector3 getRotationVector = childLC.ReturnNormalDir();

        float angle = Vector3.Angle(getRotationVector, nomalDir);

        if (transform_z == 0 && transform_x == 0)
        {
            dirpos = 5;
        }
        else
        {

            if (nomalDir.x + margin_of_error < getRotationVector.x) // ĳ������ x �̵��� ���� (7, 4, 1)
            {
                if (nomalDir.z + margin_of_error < getRotationVector.z) // ĳ������ z �̵��� �� (1, 2, 3)
                {
                    dirpos = 1;
                }
                else if (nomalDir.z + margin_of_error == 0) // ĳ������ z �̵��� ���� (4, 5, 6)
                {
                    dirpos = 4;
                }
                else // ĳ������ z �̵��� �� (7, 8, 9)
                {
                    dirpos = 7;
                }
            }
            else if (nomalDir.x + margin_of_error == 0) // ĳ������ x �̵� ���� (8, 5, 2)
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
            else // ĳ������ x �̵��� ������ (9, 6, 3)
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
        }
        animator.SetInteger("MovePos", dirpos);
        
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

    void AlertToEnemy() // ���� �� ��, 20f ���� ��, ������ ������ ��ġ�� �˸��� �Լ�.
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
