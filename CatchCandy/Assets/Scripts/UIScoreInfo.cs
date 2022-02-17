    using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreInfo : MonoBehaviour
{
    public TextMeshProUGUI Score1;
    public TextMeshProUGUI Score2;
    public TextMeshProUGUI Score3;
    public TextMeshProUGUI Score4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int scoreNorth, int scoreEast, int scoreSouth, int scoreWest)
    {
        Score1.text = "X" + scoreNorth;
        Score2.text = "X" + scoreEast;
        Score3.text = "X" + scoreSouth;
        Score4.text = "X" + scoreWest;
    }


    public void UpdateScore1(int score)
    {
        Score1.text = "X" + score;
    }

    public void UpdateScore2(int score)
    {
        Score2.text = "X" + score;
    }

    public void UpdateScore3(int score)
    {
        Score3.text = "X" + score;
    }

    public void UpdateScore4(int score)
    {
        Score4.text = "X" + score;
    }


}
