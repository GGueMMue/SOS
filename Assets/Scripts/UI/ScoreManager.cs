using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int scores = 0;
    public Text score_Text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (scores == 0)
            score_Text.text = "SCORE: 000";

        else
            score_Text.text = "SCORE: " + scores;
    }
}
