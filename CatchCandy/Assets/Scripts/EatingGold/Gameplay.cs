using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EatingGold
{

    public enum RoundState
    {
        RoundCheck,
        PreRound,
        Rounding,
        PostRound,
        GameFinished
    }

    public class Gameplay : MonoBehaviour
    {
        public int RoundLimit = 5;
        public GoldControl GoldControl;

        public UIGameInfo GameInfo;
        public UIRoundInfo RoundInfo;
        public UIScoreInfo ScoreInfo;
        public UICountDownInfo CountDownInfo;

        int RoundNum = -1;
        RoundState State = RoundState.RoundCheck;

        public PlayerFSM PlayerFSMNorth;
        public PlayerFSM PlayerFSMEast;
        public PlayerFSM PlayerFSMSouth;
        public PlayerFSM PlayerFSMWest;

        public int RoundSeconds = 10;

        int ScoreNorth = 0;
        int ScoreEast = 0;
        int ScoreSouth = 0;
        int ScoreWest = 0;


        bool PreRoundStart = false;
        bool RoundingStart = false;
        bool PostRoundStart = false;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            switch (State)
            {
                case RoundState.RoundCheck:
                    RoundCheck();

                    break;
                case RoundState.PreRound:
                    if (!PreRoundStart)
                    {
                        StartCoroutine(PreRound());
                    }
                    break;
                case RoundState.Rounding:
                    if (!RoundingStart)
                    {
                        StartCoroutine(Rounding());
                    }
                    break;
                case RoundState.PostRound:
                    if (!PostRoundStart)
                    {
                        StartCoroutine(PostRound());
                    }
                    break;
                case RoundState.GameFinished:
                    return;
            }
        }

        IEnumerator PreRound()
        {
            PreRoundStart = true;

            yield return new WaitForSeconds(1.2f);

            RoundInfo.ShowRound("Round  " + (RoundNum + 1));
            yield return new WaitForSeconds(1.0f);

            GoldControl.GenerateGold(RoundNum);

            State = RoundState.Rounding;
        }

        IEnumerator Rounding()
        {
            RoundingStart = true;

            PlayerFSMNorth.StartSelecting();
            PlayerFSMEast.StartSelecting();
            PlayerFSMSouth.StartSelecting();
            PlayerFSMWest.StartSelecting();

            System.Random rand = new System.Random(Time.frameCount);
            float randomEast = rand.Next(RoundSeconds);
            float randomNorth = rand.Next(RoundSeconds);
            float randomSouth = rand.Next(RoundSeconds);
            CountDownInfo.CountDown(RoundSeconds);
            float time = 0;
            while (time < RoundSeconds &&
                (PlayerFSMNorth.currentState.ToString().Equals(PlayerFSM.PlayerState.Start.ToString()) ||
                PlayerFSMEast.currentState.ToString().Equals(PlayerFSM.PlayerState.Start.ToString()) ||
                PlayerFSMSouth.currentState.ToString().Equals(PlayerFSM.PlayerState.Start.ToString()) ||
                PlayerFSMWest.currentState.ToString().Equals(PlayerFSM.PlayerState.Start.ToString())))
            {
                time += Time.deltaTime;

                // random ai
                if (time > randomEast && PlayerFSMEast.currentState.ToString().Equals(PlayerFSM.PlayerState.Start.ToString()))
                {
                    PlayerFSMEast.Select((PlayerFSM.Selection)rand.Next(3));
                }
                if (time > randomNorth && PlayerFSMNorth.currentState.ToString().Equals(PlayerFSM.PlayerState.Start.ToString()))
                {
                    PlayerFSMNorth.Select((PlayerFSM.Selection)rand.Next(3));
                }
                if (time > randomSouth && PlayerFSMSouth.currentState.ToString().Equals(PlayerFSM.PlayerState.Start.ToString()))
                {
                    PlayerFSMSouth.Select((PlayerFSM.Selection)rand.Next(3));
                }

                yield return null;
            }

            CountDownInfo.Dissmiss();
            yield return new WaitForSeconds(0.7f);

            State = RoundState.PostRound;
        }

        IEnumerator PostRound()
        {
            PostRoundStart = true;

            // Calculate score
            int cloudSelNorth = PlayerFSMNorth.GetSelectedCloud();
            int cloudSelSouth = PlayerFSMSouth.GetSelectedCloud();
            int cloudSelWest = PlayerFSMWest.GetSelectedCloud();
            int cloudSelEast = PlayerFSMEast.GetSelectedCloud();

            int scoreDeltaNorth = 0;
            int scoreDeltaSouth = 0;
            int scoreDeltaWest = 0;
            int scoreDeltaEast = 0;

            if (cloudSelNorth != cloudSelSouth && cloudSelNorth != cloudSelWest && cloudSelNorth != cloudSelEast)
            {
                scoreDeltaNorth = GoldControl.GetGoldNum(RoundNum, cloudSelNorth);
            }
            if (cloudSelNorth != cloudSelSouth && cloudSelSouth != cloudSelWest && cloudSelSouth != cloudSelEast)
            {
                scoreDeltaSouth = GoldControl.GetGoldNum(RoundNum, cloudSelSouth);
            }
            if (cloudSelWest != cloudSelSouth && cloudSelNorth != cloudSelWest && cloudSelWest != cloudSelEast)
            {
                scoreDeltaWest = GoldControl.GetGoldNum(RoundNum, cloudSelWest);
            }
            if (cloudSelEast != cloudSelSouth && cloudSelEast != cloudSelWest && cloudSelNorth != cloudSelEast)
            {
                scoreDeltaEast = GoldControl.GetGoldNum(RoundNum, cloudSelEast);
            }
            ScoreEast += scoreDeltaEast;
            ScoreNorth += scoreDeltaNorth;
            ScoreWest += scoreDeltaWest;
            ScoreSouth += scoreDeltaSouth;

            // player rotate andd jump animation
            PlayerFSMEast.RotateToSel();
            PlayerFSMNorth.RotateToSel();
            PlayerFSMWest.RotateToSel();
            PlayerFSMSouth.RotateToSel();
            yield return new WaitForSeconds(0.2f);

            PlayerFSMEast.JumpToSel();
            PlayerFSMNorth.JumpToSel();
            PlayerFSMWest.JumpToSel();
            PlayerFSMSouth.JumpToSel();
            yield return new WaitForSeconds(0.5f);

            // gold dismiss and gold eating ui
            if (scoreDeltaNorth > 0)
            {
                GoldControl.DismissGold(cloudSelNorth);
            }
            if (scoreDeltaSouth > 0)
            {
                GoldControl.DismissGold(cloudSelSouth);
            }
            if (scoreDeltaWest > 0)
            {
                GoldControl.DismissGold(cloudSelWest);
            }
            if (scoreDeltaEast > 0)
            {
                GoldControl.DismissGold(cloudSelEast);
            }
            ScoreInfo.UpdateScore(ScoreNorth, ScoreEast, ScoreSouth, ScoreWest);
            yield return new WaitForSeconds(1.0f);

            // show result animation
            PlayerFSMEast.ShowResult(scoreDeltaEast);
            PlayerFSMNorth.ShowResult(scoreDeltaNorth);
            PlayerFSMWest.ShowResult(scoreDeltaWest);
            PlayerFSMSouth.ShowResult(scoreDeltaSouth);
            yield return new WaitForSeconds(3.2f);

            // rotate and jump back
            if (scoreDeltaNorth > 0)
            {
                PlayerFSMNorth.RotateBack();
            }
            if (scoreDeltaSouth > 0)
            {
                PlayerFSMSouth.RotateBack();
            }
            if (scoreDeltaWest > 0)
            {
                PlayerFSMWest.RotateBack();
            }
            if (scoreDeltaEast > 0)
            {
                PlayerFSMEast.RotateBack();
            }
            yield return new WaitForSeconds(0.5f);

            if (scoreDeltaNorth > 0)
            {
                PlayerFSMNorth.JumpBack();
            }
            if (scoreDeltaSouth > 0)
            {
                PlayerFSMSouth.JumpBack();
            }
            if (scoreDeltaWest > 0)
            {
                PlayerFSMWest.JumpBack();
            }
            if (scoreDeltaEast > 0)
            {
                PlayerFSMEast.JumpBack();
            }
            yield return new WaitForSeconds(1.0f);

            // rotate to default
            PlayerFSMEast.RotateToDefault();
            PlayerFSMNorth.RotateToDefault();
            PlayerFSMWest.RotateToDefault();
            PlayerFSMSouth.RotateToDefault();
            yield return new WaitForSeconds(0.5f);

            // return idle animation
            PlayerFSMEast.Idle();
            PlayerFSMNorth.Idle();
            PlayerFSMWest.Idle();
            PlayerFSMSouth.Idle();

            GoldControl.DestroyAllGolds();
            yield return new WaitForSeconds(1);
            State = RoundState.RoundCheck;
        }

        void RoundCheck()
        {
            RoundNum++;
            if (RoundNum == 0)
            {
                GameStart();
            }
            if (RoundNum == RoundLimit)
            {
                GameOver();
                State = RoundState.GameFinished;
                return;
            }
            State = RoundState.PreRound;

            PreRoundStart = false;
            RoundingStart = false;
            PostRoundStart = false;
        }

        void GameOver()
        {
            GameInfo.ShowQuickStart("Finished!");
        }

        void GameStart()
        {
            GameInfo.ShowQuickStart("Start!");
        }

    }
}
