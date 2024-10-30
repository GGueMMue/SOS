using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    int max_value = 700;
    int possible_max_score;

    // Start is called before the first frame update
    void Start()
    {
        possible_max_score = max_value * DataForScoreCalculator.TOTAL_ENEMY;
    }

    public string ReturningPlayerRank()
    {
        if (DataForScoreCalculator.PLAYER_TOTAL_SCORE >= (possible_max_score * 90) / 100) return "S";
        else if (DataForScoreCalculator.PLAYER_TOTAL_SCORE >= (possible_max_score * 80) / 100) return "A";
        else if (DataForScoreCalculator.PLAYER_TOTAL_SCORE >= (possible_max_score * 70) / 100) return "B";
        else if (DataForScoreCalculator.PLAYER_TOTAL_SCORE >= (possible_max_score * 60) / 100) return "C";
        else return "D";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
