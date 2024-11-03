using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class StageData // 게임 저장 데이터, List로 사용
{
    public string stageName;
    public int clearScore;
    public string clearRank;
}

[System.Serializable]
public class GameSaveData
{
    public List<StageData> clearData = new List<StageData>(); 
}






public class SaveGameDataManager : MonoBehaviour
{
    public static GameSaveData saveData;
    public string filePath;

    public GameObject rankPrintGameObject;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        // 경로 지정

        LoadGameData();
    }

    void LoadGameData()
    {
        if(File.Exists(filePath))
        {
            string jsonClearData = File.ReadAllText(filePath);
            saveData = JsonUtility.FromJson<GameSaveData>(jsonClearData);
        }
        else
        {
            saveData = new GameSaveData();
        }
    }

    public void SaveNowStageData()
    {
        string curStage = DataForScoreCalculator.NOW_STAGE;
        int curScore = DataForScoreCalculator.PLAYER_TOTAL_SCORE;
        string curRank = rankPrintGameObject.GetComponent<Text>().text;

        Debug.Log($"Current Stage: {curStage}, Score: {curScore}, Rank: {curRank}");

        StageData beforeRecord = saveData.clearData.Find(listRecord => listRecord.stageName == curStage);

        if (beforeRecord == null) // 첫 클리어?
        {
            StageData newRecord = new StageData
            {
                stageName = curStage,
                clearRank = curRank,
                clearScore = curScore
            };

            saveData.clearData.Add(newRecord);
            Debug.Log("New record added to saveData.");
        }
        else if (curScore > beforeRecord.clearScore) // 기존 기록보다 높은가?
        {
            beforeRecord.clearScore = curScore;
            beforeRecord.clearRank = curRank;
            Debug.Log("Existing record updated in saveData.");
        }
        else
        {
            Debug.Log("Score is not higher than existing record; no update needed.");
        }

        // JSON 직렬화 및 파일 저장
        string jsonData = JsonUtility.ToJson(saveData, true);
        Debug.Log($"JSON Data to be saved: {jsonData}");

        File.WriteAllText(filePath, jsonData);
        Debug.Log("Data saved to JSON file.");
    }

    public static StageData GetStageRecord(string findStageName) // 스타트 화면에서 사용 예정 스테이지 이름 찾기
    {
        if (saveData == null) return null;
        return saveData.clearData.Find(listRecord => listRecord.stageName == findStageName);
    }


    // Start is called before the first frame update

    IEnumerator Start()
    {

        if (rankPrintGameObject != null)
        {
            var scoreCalculator = rankPrintGameObject.GetComponent<ScoreCalculator>();
            yield return new WaitUntil(() => scoreCalculator.isInit); // 텍스트가 완전히 변경 된 이후에 실행
        }
        if (rankPrintGameObject != null) SaveNowStageData();
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
