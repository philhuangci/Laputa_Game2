using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreBil : MonoBehaviour
{
    public Text Player1;
    public Text Player2;
    public Text Player3;
    public Text Player4;

    public Transform Player1Trans;
    public Transform Player2Trans;
    public Transform Player3Trans;
    public Transform Player4Trans;

    public Vector2 Offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(Player1Trans.position);
        Player1.rectTransform.position = screenPos + Offset;

        screenPos = Camera.main.WorldToScreenPoint(Player2Trans.position);
        Player2.rectTransform.position = screenPos + Offset;

        screenPos = Camera.main.WorldToScreenPoint(Player3Trans.position );
        Player3.rectTransform.position = screenPos + Offset;

        screenPos = Camera.main.WorldToScreenPoint(Player4Trans.position);
        Player4.rectTransform.position = screenPos + Offset;
    }

    public void UpdateScore(int player1, int player2, int player3, int player4)
    {
        Player1.text = "+" + player1;
        Player2.text = "+" + player2;
        Player3.text = "+" + player3;
        Player4.text = "+" + player4;
        if(player1 > 0)
        {
            StartCoroutine(Player1Add());
        }
        if(player2 > 0)
        {
            StartCoroutine(Player2Add());
        }
        if(player3 > 0)
        {
            StartCoroutine(Player3Add());
        }
        if(player4 > 0)
        {
            StartCoroutine(Player4Add());
        }
    }

    public void SubPlayer1(int score)
    {
        Player1.text = "-" + score;
        StartCoroutine(Player1Add());
    }

    public void SubPlayer2(int score)
    {
        Player2.text = "-" + score;
        StartCoroutine(Player2Add());
    }

    public void SubPlayer3(int score)
    {
        Player3.text = "-" + score;
        StartCoroutine(Player3Add());
    }

    public void SubPlayer4(int score)
    {
        Player4.text = "-" + score;
        StartCoroutine(Player4Add());
    }

    IEnumerator Player1Add()
    {
        Player1.enabled = true;
        yield return new WaitForSeconds(1.5f);
        Player1.enabled = false;
    }

    IEnumerator Player2Add()
    {
        Player2.enabled = true;
        yield return new WaitForSeconds(1.5f);
        Player2.enabled = false;
    }

    IEnumerator Player3Add()
    {
        Player3.enabled = true;
        yield return new WaitForSeconds(1.5f);
        Player3.enabled = false;
    }

    IEnumerator Player4Add()
    {
        Player4.enabled = true;
        yield return new WaitForSeconds(1.5f);
        Player4.enabled = false;
    }
}
