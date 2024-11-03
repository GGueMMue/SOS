using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSaveData : MonoBehaviour
{
    string curStageName;
    public Text rankText;
    public Text scoreText;

    //public SaveGameDataManager gameDataManager;

    StageData curStageData;

    // Start is called before the first frame update
    void Start()
    {
        curStageName = this.gameObject.name;
        GetGameDatas();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetGameDatas()
    {
        curStageData = SaveGameDataManager.GetStageRecord(curStageName);
        if(curStageData == null)
        {
            rankText.text = "N";
            scoreText.text = "Score : ";
        }
        else
        {
            rankText.text = curStageData.clearRank;
            scoreText.text = "Score : " + curStageData.clearScore.ToString();
        }
    }
}
