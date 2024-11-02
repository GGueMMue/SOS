using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public GameObject bullet_Img;
    public int total_time;

    // Start is called before the first frame update
    void Start()
    {
        DataForScoreCalculator.NOW_STAGE = "";

        Cursor.visible = true;

        if (this.gameObject.CompareTag("Gun_Kill_Counter"))
        {
            total_time = DataForScoreCalculator.GUN_KILL_COUNT;
        }
        if (this.gameObject.CompareTag("Meele_Kill_Counter"))
        {
            total_time = DataForScoreCalculator.MEELE_KILL_COUNT;
        }
        if (this.gameObject.CompareTag("Kill_Confrim_Counter"))
        {
            total_time = DataForScoreCalculator.TOTAL_KILL_CONFIRM;
        }

        StartCoroutine(TimeCancler());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TimeCancler()
    {
        for (int time = 0; time < total_time; time++)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject b_Img = Instantiate(bullet_Img);
            //RectTransform b_Rect = b_Img.GetComponent<RectTransform>();

            b_Img.transform.parent = this.gameObject.transform;
            //// 앵커를 middle-left로 설정
            //b_Rect.anchorMin = new Vector2(0, 0.5f);
            //b_Rect.anchorMax = new Vector2(0, 0.5f);

            //// 위치를 초기화
            //b_Rect.anchoredPosition = Vector2.zero;

            b_Img.transform.rotation = Quaternion.identity;
            b_Img.transform.position = new Vector3(
            this.gameObject.transform.position.x + (time * 25),
            this.gameObject.transform.position.y,
            this.gameObject.transform.position.z);
        }

        yield return null;

    }
}
