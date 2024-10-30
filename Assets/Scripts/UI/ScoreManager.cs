using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int scores = 0;
    public Text score_Text;

    // Start is called before the first frame update
    void Awake()
    {
        InitDataForScoreCalculator();
    }

    // Update is called once per frame
    void Update()
    {
        if (scores == 0)
            score_Text.text = "SCORE: 000";

        else
            score_Text.text = "SCORE: " + scores;

        DataForScoreCalculator.PLAYER_TOTAL_SCORE = scores;
    }

    void InitDataForScoreCalculator()
    {
        DataForScoreCalculator.MEELE_KILL_COUNT = 0;
        DataForScoreCalculator.TOTAL_ENEMY = 0;
        DataForScoreCalculator.TOTAL_KILL_CONFIRM = 0;
        DataForScoreCalculator.GUN_KILL_COUNT = 0;
        DataForScoreCalculator.PLAYER_TOTAL_SCORE = 0;
    }
}
