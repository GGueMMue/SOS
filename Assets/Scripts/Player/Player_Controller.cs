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
    public bool coroutineChecker = false;

    public float margin_of_error = 0.3f;

    public bool isMelee = true;
    public                                      bool possible_Kill_Confirm = false;

    public                                      bool nowReroadingRunAnimation = false;

    public int dirpos = 0;
    public CrossHair crosshair;

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
        gun = GetComponentInChildren<Gun>();

        /*
               ���� �ʿ� ����
        1.
               Gun�� start���� GetComponentInChildren���� �޴´�.
                Gun�� null�̸�, �ָ� �и� ���� �̺�Ʈ ����
                Gun�� Meele�� ���� ���� �̺�Ʈ ����(�º� has_meele_item = true)
                Gun�� �ѱ��, �� �ѱ⺰ ���� �̺�Ʈ  ����.
                ���� ������ ����, �ִϸ������� �Ķ���� bool ���� ����.

                => Ŭ����

        2.     
                �� ���⺰, �����ɽ�Ʈ ���.
                ���� ���ݰ� ������ ������ �ѱ� ����� �����ɽ�Ʈ�� �̷��� ��� �Լ��� �ۼ��� �����Ѵ�.
                �� �ѱ⺰ ��Ÿ��� ������ �ε����Ѵ�.

                => Ŭ����

        3.      
                ����
                ���� ������, ���� ������ �ִ� �ѱ⸦ ������.
                ���� �ѱ��, ��ü ���� Ins �ϰ�, ������ �̸� GŰ�� ���� �Դ´�.
                ����, �ѱ⸦ ������ �ִٸ�, �� ����� ��ü�Ѵ�.
                ��ü���� ��, ��ȹ���� �ۼ��� ������� �ѱ��� ź�� ��ź�� �ο� �޴´�.       

        4.
                ��ݿ� �����ϱ� ����, ���� ���콺 ����Ʈ�� ��ġ�� ���� ũ�ν���� �׸��� ��ġ�Ѵ�.

        5.
                ���⸦ ����� ��, ��������Ʈ�� ���带 ����ϵ��� �Ѵ�.

        6.
                ���� ��ô
                �÷��̾ ���콺 ��Ŭ������ ���� ��, ũ�ν������ �������� �ٲ��, �� ���¿��� ��Ŭ���� ������ ��, �ش� ���⿡ RigidBody�� �޸���, �߷� ������ fasle��, Addforce�� ����Ͽ� ���� �������� �ѱⰡ ���󰡵��� �Ѵ�.
                �ش� �������� ���� ���Ⱑ, ������ ������ �� ���. �ݶ��̴��� �浹 ������ ������, �߷� ������ true�� ���� ���Ⱑ ������ ���������� �Ѵ�.
                ���� ������ ����� ���Ѱ� ���õ� �Լ��� ����Ͽ� �ڽŰ� ������(ź, ��ź ��) ���⸦ ins�ϰ� distroy �Ѵ�.

        7.      
                ���� ������ ������ BGM�� �ִ´�.
         
        8.      
                ī�޶��� �̵�
                ī�޶��� �̵��� ��ȹ���� �ִ� ��� �����Ѵ�. �㳪, �ش� ������ ���� �÷��̿� �߿�ġ ���� ����̱⿡, �������� �����ϵ��� �Ѵ�.
        
         */
    }

    // Update is called once per frame
    void Update()
    {

        timechecker += Time.deltaTime;


        PlayerMove();
        childLC.PlayerRotate();//PlayerRotate(childTR);
        Debug.DrawRay(shotRocation.position, shotRocation.forward, Color.red);

        //if (Input.GetKeyDown(KeyCode.R))
        //    ReRoadingInvoke(gun.reRoadTime);

        if (gun != null)
        {
            switch (gun.gunName)
            {
                case "SMG":
                    animator.SetBool("isARorSMG", true);
                    animator.SetBool("isHG", false);
                    animator.SetBool("isSG", false);
                    animator.SetBool("Has_Meele_Item", false);

                    if (gun.Fire(timechecker, shotRocation))
                    {
                        crosshair.SetNowFireTrue();
                        animator.SetTrigger("Attack");

                        AlertToEnemy();
                        timechecker = 0;
                        Debug.Log("���");
                    }
                    crosshair.SetNowFireFalse();

                    break;

                case "Rifle":
                    animator.SetBool("isARorSMG", true);
                    animator.SetBool("isHG", false);
                    animator.SetBool("isSG", false);
                    animator.SetBool("Has_Meele_Item", false);

                    if (gun.Fire(timechecker, shotRocation))
                    {
                        crosshair.SetNowFireTrue();
                        animator.SetTrigger("Attack");

                        AlertToEnemy();
                        timechecker = 0;
                        Debug.Log("���");
                    }
                    crosshair.SetNowFireFalse();

                    break;

                case "HandGun":
                    animator.SetBool("isARorSMG", false);
                    animator.SetBool("isHG", true);
                    animator.SetBool("isSG", false);
                    animator.SetBool("Has_Meele_Item", false);

                    if (gun.Fire(timechecker, shotRocation))
                    {
                        crosshair.SetNowFireTrue();
                        animator.SetTrigger("Attack");

                        AlertToEnemy();
                        timechecker = 0;
                        Debug.Log("���");
                    }
                    crosshair.SetNowFireFalse();

                    break;

                case "Shotgun":
                    animator.SetBool("isARorSMG", false);
                    animator.SetBool("isHG", false);
                    animator.SetBool("isSG", true);
                    animator.SetBool("Has_Meele_Item", false);

                    if (gun.ShotGunFire(timechecker, shotRocation))
                    {
                        crosshair.SetNowFireTrue();
                        animator.SetTrigger("Attack");

                        AlertToEnemy();
                        timechecker = 0;
                        Debug.Log("���");
                    }
                    crosshair.SetNowFireFalse();

                    break;

                case "Meele":
                    animator.SetBool("isARorSMG", false);
                    animator.SetBool("isHG", false);
                    animator.SetBool("isSG", false);
                    animator.SetBool("Has_Meele_Item", true);

                    if (Input.GetMouseButtonDown(0))
                    {
                        animator.SetTrigger("Attack");

                        StartCoroutine(MeeleAttack());
                    }
                    break;

                default:

                    break;
            }
        }
        else
        {
            animator.SetBool("isARorSMG", false);
            animator.SetBool("isHG", false);
            animator.SetBool("isSG", false);
            animator.SetBool("Has_Meele_Item", false);

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack");

                StartCoroutine(NoneItemMeeleAttack());
            }
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

    private IEnumerator MeeleAttack()
    {
        if (!coroutineChecker)
        {
            coroutineChecker = true;
            crosshair.SetNowFireTrue();

            // �ִϸ��̼��� ���� ������ ��ٸ� (������ ��� �ð�)
            yield return new WaitForSeconds(1.1f);
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, 3.2f, Vector3.up, LayerMask.GetMask("Enemy", "Wall"));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<FSM>().SetStateDead();
                }
            }

            coroutineChecker = false;
            crosshair.SetNowFireFalse();

        }
        else
        {
            crosshair.SetNowFireFalse();

            yield return null;
        }

    }

    private IEnumerator NoneItemMeeleAttack()
    {
        if (!coroutineChecker)
        {
            coroutineChecker = true;
            crosshair.SetNowFireTrue();

            // �ִϸ��̼��� ���� ������ ��ٸ� (������ ��� �ð�)
            yield return new WaitForSeconds(0.45f);
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, 2.2f, Vector3.up, LayerMask.GetMask("Enemy", "Wall"));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<FSM>().SetStateDead();
                }
            }

            coroutineChecker = false;
            crosshair.SetNowFireFalse();
        }
        else
        {
            crosshair.SetNowFireFalse();

            yield return null;
        }

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
        } // 1��*/

        Vector3 playerMoveDir = transform.TransformDirection(new Vector3(transform_x, 0, transform_z)).normalized;

        //Vector3 playerMoveDir = new Vector3(transform_x, 0, transform_z); // �÷��̾� �̵� ����
        Vector3 getRotationVector = childLC.ReturnDir(); // ĳ������ ���� �ٶ󺸴� ���� ����


        // �̵��� ���� ���� ������ ���
        if (playerMoveDir != Vector3.zero)
        {
            float angle = Vector3.SignedAngle(getRotationVector, playerMoveDir, Vector3.up);

            // ������� ���� �α�
            //Debug.Log($"Move Direction: {playerMoveDir}, Rotation: {getRotationVector}, Angle: {angle}");

            // ������ ����� ��ȯ (0~360��)
            if (angle < 0)
                angle += 360;

            // 8���� ����
            if (angle >= 337.5f || angle < 22.5f)
            {
                dirpos = 8; // ����
            }
            else if (angle >= 22.5f && angle < 67.5f)
            {
                dirpos = 9; // ���� �밢�� ��
            }
            else if (angle >= 67.5f && angle < 112.5f)
            {
                dirpos = 6; // ������
            }
            else if (angle >= 112.5f && angle < 157.5f)
            {
                dirpos = 3; // ���� �밢�� ��
            }
            else if (angle >= 157.5f && angle < 202.5f)
            {
                dirpos = 2; // ����
            }
            else if (angle >= 202.5f && angle < 247.5f)
            {
                dirpos = 1; // ���� �밢�� ��
            }
            else if (angle >= 247.5f && angle < 292.5f)
            {
                dirpos = 4; // ����
            }
            else if (angle >= 292.5f && angle < 337.5f)
            {
                dirpos = 7; // ���� �밢�� ��
            }

            // ������ ����ؼ� 360�� �������� ������ ������.
            /*float angle = Vector3.SignedAngle(getRotationVector, playerMoveDir, Vector3.up);

            Debug.DrawRay(transform.position, playerMoveDir * 2, Color.red);
            Debug.DrawRay(transform.position, getRotationVector * 2, Color.blue);

            //Debug.Log(angle);

            // ������ ���� 8�������� ������
            if (angle >= -22.5f && angle < 22.5f)
            {
                dirpos = 8; // ���� (��)
            }
            else if (angle >= 22.5f && angle < 67.5f)
            {
                dirpos = 9; // ���� �밢�� ��
            }
            else if (angle >= 67.5f && angle < 112.5f)
            {
                dirpos = 6; // ������
            }
            else if (angle >= 112.5f && angle < 157.5f)
            {
                dirpos = 3; // ���� �밢�� ��
            }
            else if ((angle >= 157.5f && angle <= 180f) || (angle < -157.5f && angle >= -180f))
            {
                dirpos = 2; // ����
            }
            else if (angle >= -157.5f && angle < -112.5f)
            {
                dirpos = 1; // ���� �밢�� ��
            }
            else if (angle >= -112.5f && angle < -67.5f)
            {
                dirpos = 4; // ����
            }
            else if (angle >= -67.5f && angle < -22.5f)
            {
                dirpos = 7; // ���� �밢�� ��
            } // 2-1�� */
        }
        else
        {
            dirpos = 5; // �̵����� ���� �� (���� ����)
        } // 2�� ȸ���� ���*/


        animator.SetInteger("MovePos", dirpos);

        // �̵� ����
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
