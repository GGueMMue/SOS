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
