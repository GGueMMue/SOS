using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Camera_MoveMent : MonoBehaviour
{
    Transform                               playerTr;
    BoxCollider                             MovementCheckerBoxColider;


    public Vector3                                 startPos;
    public Vector3                                 newPos;

    public bool                             checker = false;

    public float                            fixed_Y = 10f;

    public  float value = 0.125f;
    // Start is called before the first frame update
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        MovementCheckerBoxColider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checker)
        {
            Vector3 desiredPos = new Vector3(playerTr.position.x, fixed_Y, playerTr.position.z);
            // XZ 축에 대해서만 부드럽게 보간 (Y는 고정)
            newPos = Vector3.Lerp(transform.position, desiredPos, 0.5f);

            // 카메라의 위치를 업데이트 (Y축 고정)
            transform.position = new Vector3(newPos.x, fixed_Y, newPos.z);
        }
    } // 죽여벌랑 진짜

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            checker = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        startPos = this.gameObject.transform.position;
        if(other.gameObject.CompareTag("Player"))
        {
            checker = true;
        }
    }
}
