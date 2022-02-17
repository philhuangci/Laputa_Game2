using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruit
{
    public class Gameplay : MonoBehaviour
    {
        public UIScoreInfo ScoreInfo;
        public UIScoreBil ScoreBil;
        public UICountDownInfo CountInfo;
        public UIGameInfo GameInfo;

        public FruitFactory FruitFactory;

        public PlayerFSM Player1;
        public PlayerFSM Player2;
        public PlayerFSM Player3;
        public PlayerFSM Player4;

        int Score1;
        int Score2;
        int Score3;
        int Score4;

        public AudioSource BGMAudioSource;
        private AudioSource m_AudioSource;
        public  AudioClip GameStartSound;
        public  AudioClip GameFinishSound;
        public AudioClip GameFinishMusic;
        

        public int GameLengthInSec = 10;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(GameStart());
            Player1.OnGetFruit = AddScore;
            Player2.OnGetFruit = AddScore;
            Player3.OnGetFruit = AddScore;
            Player4.OnGetFruit = AddScore;

            Player1.OnGetBoom = SubScore;
            Player2.OnGetBoom = SubScore;
            Player3.OnGetBoom = SubScore;
            Player4.OnGetBoom = SubScore;

            m_AudioSource = GetComponent<AudioSource>();

        }

        private void PlayAudio(AudioClip audioClip)
        {
            m_AudioSource.clip = audioClip;
            m_AudioSource.Play();
        }


        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator GameStart()
        {
            yield return new WaitForSeconds(1);
            GameInfo.ShowQuickStart("Start!");
            PlayAudio(GameStartSound);
            yield return new WaitForSeconds(0.5f);
            FruitFactory.StartGenFruit();
            Player1.StartGame();
            Player2.StartGame();
            Player3.StartGame();
            Player4.StartGame();
            CountInfo.CountDown(GameLengthInSec);

            yield return new WaitForSeconds(GameLengthInSec - 3);
            FruitFactory.StopGenFruit();

            yield return new WaitForSeconds(3);
            Player1.StopGame();
            Player2.StopGame();
            Player3.StopGame();
            Player4.StopGame();

            GameInfo.ShowQuickStart("Finished!");
            PlayAudio(GameFinishSound);
            CountInfo.Dissmiss();
            BGMAudioSource.Pause();
            yield return new WaitForSeconds(1.0f);

            PlayAudio(GameFinishMusic);

            yield return new WaitForSeconds(3.0f);


        }

        public void AddScore(int playerid, int score)
        {
            if(playerid == 1)
            {
                Score1 += score;
                ScoreInfo.UpdateScore1(Score1);
                UpdatePlayer(playerid, Score1);
            }
            else if(playerid == 2)
            {
                Score2 += score;
                ScoreInfo.UpdateScore2(Score2);
                UpdatePlayer(playerid, Score2);
            }
            else if(playerid == 3)
            {
                Score3 += score;
                ScoreInfo.UpdateScore3(Score3);
                UpdatePlayer(playerid, Score3);
            }
            else
            {
                Score4 += score;
                ScoreInfo.UpdateScore4(Score4);
                UpdatePlayer(playerid, Score4);
            }
        }

        public void SubScore(int playerid, int score)
        {
            if (playerid == 1)
            {
                Score1 = Mathf.Max(0, Score1 - score);
                ScoreInfo.UpdateScore1(Score1);
                ScoreBil.SubPlayer1(score);
                UpdatePlayer(playerid, Score1);
            }
            else if (playerid == 2)
            {
                Score2 = Mathf.Max(0, Score2 - score);
                ScoreInfo.UpdateScore2(Score2);
                ScoreBil.SubPlayer2(score);
                UpdatePlayer(playerid, Score2);
            }
            else if (playerid == 3)
            {
                Score3 = Mathf.Max(0, Score3 - score);
                ScoreInfo.UpdateScore3(Score3);
                ScoreBil.SubPlayer3(score);
                UpdatePlayer(playerid, Score3);
            }
            else
            {
                Score4 = Mathf.Max(0, Score4 - score);
                ScoreInfo.UpdateScore4(Score4);
                ScoreBil.SubPlayer4(score);
                UpdatePlayer(playerid, Score4);
            }
        }

        public void UpdatePlayer(int playerid, int score)
        {
            if(playerid == 1)
            {
                Player1.UpdateCandies(score);
            }
            else if (playerid == 2)
            {
                Player2.UpdateCandies(score);
            }
            else if(playerid == 3)
            {
                Player3.UpdateCandies(score);
            }
            else
            {
                Player4.UpdateCandies(score);
            }
        }
    }
}
