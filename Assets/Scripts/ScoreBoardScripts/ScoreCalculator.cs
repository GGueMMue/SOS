using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCalculator : MonoBehaviour
{
    int max_value = 700;
    int possible_max_score;

    public Score_Printer printer;

    Text text;

    // Start is called before the first frame update
    void Start()
    {
        possible_max_score = max_value * DataForScoreCalculator.TOTAL_ENEMY;
        text = GetComponent<Text>();
        text.enabled = false;
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
        if (printer.endofcal)
        {
            text.enabled = true;
            text.text = ReturningPlayerRank();
        }
    }
}
