using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class ClearChecker : MonoBehaviour
{
    [SerializeField] List<GameObject> Enemy_List;
    [SerializeField] BoxCollider clearColider;

    public GameObject clearPointBox;

    [SerializeField] Camera mainCamera;
    [SerializeField] Transform clearTr;
    [SerializeField] Transform playerTr;

    public RectTransform dirUI;

    public bool isClear = false;

    // Start is called before the first frame update
    void Start()
    {
        Enemy_List = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        
        DataForScoreCalculator.TOTAL_ENEMY = Enemy_List.Count;
        DataForScoreCalculator.NOW_STAGE = SceneManager.GetActiveScene().name;

        Debug.Log(DataForScoreCalculator.NOW_STAGE);
        clearColider = clearPointBox.GetComponent<BoxCollider>();
        clearTr = clearPointBox.GetComponent<Transform>();
        clearPointBox.SetActive(false);

        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        
    }

    public void RemoveListEnemy(GameObject enemy)
    {
        if (enemy.GetComponent<FSM>().state == FSM.STATE.DEAD)
        {
            Enemy_List.Remove(enemy);

            if (Enemy_List.Count <= 0)
            {
                clearPointBox.SetActive(true);
                SetClearPointOutline();
                isClear = true;
            }
        }
    }

    public void ShowNavDir()
    {
        // 클리어 지점과 플레이어의 위치를 동일한 y값으로 맞춤
        Vector3 ySetValueOfClearTr = new Vector3(clearTr.position.x, playerTr.position.y, clearTr.position.z);

        // 월드 스페이스 상의 방향
        Vector3 worldDir = (ySetValueOfClearTr - playerTr.position).normalized;

        // 타겟의 스크린 좌표
        Vector3 screenPos = mainCamera.WorldToScreenPoint(ySetValueOfClearTr);
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        if (screenPos.x < 0 || screenPos.x > Screen.width ||
           screenPos.y < 0 || screenPos.y > Screen.height)
        {
            dirUI.gameObject.SetActive(true);

            // 스크린 상에서의 방향 계산
            Vector2 screenDir = new Vector2(screenPos.x - screenCenter.x, screenPos.y - screenCenter.y).normalized;

            // 각도 계산 (스크린 좌표계 기준)
            float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg;

            // UI 회전 설정 (기본이 위쪽을 향하므로 -90도 보정)
            dirUI.rotation = Quaternion.Euler(0, 0, angle - 90);

            // UI 위치 설정
            //float offset = 100f; // 화면 가장자리로부터의 거리
            Vector2 viewportPoint = new Vector2(
                Mathf.Clamp(screenPos.x / Screen.width, 0.1f, 0.9f),
                Mathf.Clamp(screenPos.y / Screen.height, 0.1f, 0.9f));

            dirUI.position = new Vector3(
                viewportPoint.x * Screen.width,
                viewportPoint.y * Screen.height,
                0);
        }
        else
        {
            dirUI.gameObject.SetActive(false);
        }
    }

    public void SetClearPointOutline()
    {
        LineRenderer line = clearPointBox.AddComponent<LineRenderer>();

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.green;
        line.endColor = Color.green;
        line.startWidth = .1f;
        line.endWidth = .1f;

        Vector3 size = clearPointBox.GetComponent<BoxCollider>().size;
        Vector3 position = clearPointBox.transform.position;

        Vector3[] points = new Vector3[16];

        points[0] = position + new Vector3(-size.x, -size.y, -size.z);
        points[1] = position + new Vector3(size.x, -size.y, -size.z);
        points[2] = position + new Vector3(size.x, -size.y, size.z);
        points[3] = position + new Vector3(-size.x, -size.y, size.z);
        points[4] = position + new Vector3(-size.x, -size.y, -size.z);
        points[5] = position + new Vector3(-size.x, size.y, -size.z);
        points[6] = position + new Vector3(size.x, size.y, -size.z);
        points[7] = position + new Vector3(size.x, size.y, size.z);
        points[8] = position + new Vector3(-size.x, size.y, size.z);
        points[9] = position + new Vector3(-size.x, size.y, -size.z);
        points[10] = position + new Vector3(-size.x, size.y, size.z);
        points[11] = position + new Vector3(-size.x, -size.y, size.z);
        points[12] = position + new Vector3(size.x, -size.y, size.z);
        points[13] = position + new Vector3(size.x, size.y, size.z);
        points[14] = position + new Vector3(size.x, size.y, -size.z);
        points[15] = position + new Vector3(size.x, -size.y, -size.z);

        line.positionCount = points.Length;
        line.SetPositions(points);
    }


    // Update is called once per frame
    void Update()
    {
        if (isClear)
            ShowNavDir();
    }
}
