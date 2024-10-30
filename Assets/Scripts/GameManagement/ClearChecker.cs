using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ClearChecker : MonoBehaviour
{
    [SerializeField] List<GameObject> Enemy_List;
    public BoxCollider clearColider;

    public Camera mainCamera;
    public Transform clearTr;
    [SerializeField] Transform playerTr;

    public RectTransform dirUI;

    public bool isClear = false;

    // Start is called before the first frame update
    void Start()
    {
        Enemy_List = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        clearColider.enabled = false;
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        /* 구조. 각 스테이지 별로 클리어 시 들어갈 장소가 될 위치에, mesh 렌더러가 꺼진 박스 콜라이더를
           놓음. 이 박스 콜라이더를 해당 클리어 매니저에 지정을 함.
           게임 시작 시, 적들을 Withtag로 받아오고, 해당 적들을 전부 list에 push함.
           만약, 해당 적이 죽게 되면 FSM에서 RemoveListEnemy를 불러 자신을 찾아 Remove함.
           만약, 카운트가 0이 되면, 클리어 Colider가 켜지며, ShowNavDir을 통해 클리어 위치를
           UI로 표현해줌.
        
         
           모든 적 유닛이 죽자마자 클리어를 시키지 않는 이유?
                -> 확인사살 또한 점수가 들어오기 때문.
         */
        
    }

    public void RemoveListEnemy(GameObject enemy)
    {
        if (enemy.GetComponent<FSM>().state == FSM.STATE.DEAD)
        {
            Enemy_List.Remove(enemy);

            if (Enemy_List.Count <= 0)
            {
                clearColider.enabled = true;
                isClear = true;
            }
        }
    }

    public void ShowNavDir()
    {
        Vector3 dir = (clearTr.position - playerTr.position).normalized;
        
        Vector3 screenPos = mainCamera.WorldToScreenPoint(clearTr.position);

        if(screenPos.x < 0 || screenPos.x > Screen.width || /*x 클리어 포지션의 x 축이 벗어날 때*/
            screenPos.y < 0 || screenPos.y > Screen.height /*y 클리어 포지션의 y 축이 벗어날 때*/
            )
        {
            dirUI.gameObject.SetActive(true);

            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            dirUI.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isClear)
            ShowNavDir();
    }
}
