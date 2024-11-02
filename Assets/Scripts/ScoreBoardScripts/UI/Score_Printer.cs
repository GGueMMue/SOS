using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Score_Printer : MonoBehaviour
{
    int counter;
    Text text;
    public bool endofcal = false;
    private bool coroutineRunnerChecker = false; 

    void Start()
    {
        counter = DataForScoreCalculator.PLAYER_TOTAL_SCORE;
        text = GetComponent<Text>();
        StartCoroutine(PrintScore());
    }

    IEnumerator PrintScore()
    {
        if(coroutineRunnerChecker) yield return null;

        coroutineRunnerChecker = true;

        for(int i = 0; i <= counter; i += 100)
        {
            text.text = i.ToString();
            Debug.Log(DataForScoreCalculator.PLAYER_TOTAL_SCORE);
            yield return new WaitForSeconds(0.1f);
        }

        // 마지막 정확한 값 표시
        text.text = counter.ToString();
        endofcal = true;
        coroutineRunnerChecker = false;
    }
}
