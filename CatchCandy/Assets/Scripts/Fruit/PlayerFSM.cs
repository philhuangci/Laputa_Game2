using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Fruit
{
    public class PlayerFSM : SuperStateMachine
    {
        public enum PlayerState
        {
            Idle,
            Win,
            Defeat,
            Run,
            FallDown,
        }

        // Audio
        public AudioSource audioSource;
        public AudioSource walkAudioSource;
        public AudioClip WalkSound;
        public AudioClip HitSound;
        public AudioClip GetCandy;


        [HideInInspector]
        public float Vertical = 0;
        [HideInInspector]
        public float Horizontal = 0;

        public float TurnSpeed = 3;
        public float Speed = 20.0f;
        public float Gravity = 20;

        public int PlayerId;

        public ParticleSystem Stars;

        public bool IsAI = false;

        bool Playing = false;

        Animator Animator;

        NavMeshAgent navMeshAgent;
        Rigidbody Rigid;

        public Action<int, int> OnGetFruit;
        public Action<int, int> OnGetBoom;


        public GameObject Candy1;
        public GameObject Candy2;
        public GameObject Candy3;
        public GameObject Candy4;
        public GameObject Candy5;

        // Start is called before the first frame update
        void Start()
        {
            Animator = GetComponentInChildren<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            Rigid = GetComponent<Rigidbody>();
            currentState = PlayerState.Idle;
            walkAudioSource.clip = WalkSound;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            base.FixedUpdate();
        }

        void Translate(float hor, float ver)
        {
            Vector3 dir = new Vector3(hor, 0, ver);
            dir.Normalize();
            Vector3 transformValue = dir * Time.deltaTime * Speed;
            //transform.Translate(transformValue,Space.World);
            Rigid.MovePosition(transformValue + transform.position);
        }

        void Rotating(float hor, float ver)
        {
            Vector3 dir = new Vector3(hor, 0, ver);
            Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.deltaTime * TurnSpeed);
        }


        #region state methods
        void Idle_EnterState()
        {
            Animator.Play("idle");
        }

        void Idle_ExitState()
        {

        }

        void Idle_SuperUpdate()
        {
            if (!IsAI && Playing)
            {
                Horizontal = Input.GetAxis("Horizontal");
                Vertical = Input.GetAxis("Vertical");
                if (Horizontal != 0 || Vertical != 0)
                {
                    Rotating(Horizontal, Vertical);
                    Translate(Horizontal, Vertical);
                    currentState = PlayerState.Run;
                }
            }
        }

        void Run_EnterState()
        {
            Animator.Play("run");
            walkAudioSource.loop = true;
            walkAudioSource.Play();
        }

        void Run_ExitState()
        {
            walkAudioSource.loop = false;
            walkAudioSource.Pause();
        }

        void Run_SuperUpdate()
        {
            if (!IsAI)
            {
                Horizontal = Input.GetAxis("Horizontal");
                Vertical = Input.GetAxis("Vertical");
                if (Horizontal != 0 || Vertical != 0)
                {
                    Rotating(Horizontal, Vertical);
                    Translate(Horizontal, Vertical);
                    currentState = PlayerState.Run;
                }
                else
                {
                    currentState = PlayerState.Idle;
                }
            }
            else
            {
                if ((navMeshAgent && Vector3.Distance(navMeshAgent.destination, navMeshAgent.nextPosition) <= 0.05f) || Time.time - timeEnteredState > 3)
                {
                    navMeshAgent.SetDestination(new Vector3(UnityEngine.Random.Range(-3f, 3f), 0.33f, UnityEngine.Random.Range(-3f, 3f)));
                    timeEnteredState = Time.time;
                }
            }
        }

        void FallDown_EnterState()
        {
            if (IsAI)
            {
                navMeshAgent.isStopped = true; ;
            }
            Animator.Play("fallDown");
            // PlayAudioSource(HitSound);
        }

        void FallDown_ExitState()
        {

        }

        void FallDown_SuperUpdate()
        {
            var animatorState = Animator.GetCurrentAnimatorStateInfo(0);
            if (animatorState.IsName("fallDown"))
            {
                float length = animatorState.length;

                if (Time.time - timeEnteredState < length * 0.15f)
                {
                    transform.Translate(new Vector3(0, 0, -0.08f), Space.Self);
                }

                if (Time.time - timeEnteredState >= length)
                {
                    if (IsAI)
                    {
                        navMeshAgent.isStopped = false;
                        currentState = PlayerState.Run;
                    }
                    else
                    {
                        currentState = PlayerState.Idle;
                    }
                }
            }
        }

        void Win_EnterState()
        {
            Animator.Play("win");

        }

        void Win_ExitState()
        {

        }

        void Win_SuperUpdate()
        {

        }

        void Defeat_EnterState()
        {
            Animator.Play("defeat");
        }

        void Defeat_ExitState()
        {

        }

        void Defeat_SuperUpdate()
        {

        }

        #endregion


        public void FallDown()
        {
            currentState = PlayerState.FallDown;
        }


        public void ShowStars(Vector3 position)
        {
            Stars.transform.position = position;
            Stars.Play();
        }

        public void StartGame()
        {
            if (IsAI)
            {
                currentState = PlayerState.Run;
            }
            Playing = true;
        }

        public void StopGame()
        {
            Playing = false;
            currentState = PlayerState.Idle;

            if (navMeshAgent)
            {
                navMeshAgent.Stop();
            }
        }

        public void GetFruit()
        {
            OnGetFruit(PlayerId, 1);
            PlayAudioSource(GetCandy);
        }

        public void GetBoom()
        {
            OnGetBoom(PlayerId, 3);
            PlayAudioSource(HitSound);
        }

        public void UpdateCandies(int score)
        {
            if (score > 20)
            {
                Candy1.SetActive(true);
                Candy2.SetActive(true);
                Candy3.SetActive(true);
                Candy4.SetActive(true);
                Candy5.SetActive(true);
            }
            else if (score > 15)
            {
                Candy1.SetActive(true);
                Candy2.SetActive(true);
                Candy3.SetActive(true);
                Candy4.SetActive(true);
                Candy5.SetActive(false);
            }
            else if (score > 10)
            {
                Candy1.SetActive(true);
                Candy2.SetActive(true);
                Candy3.SetActive(true);
                Candy4.SetActive(false);
                Candy5.SetActive(false);
            }
            else if (score > 5)
            {
                Candy1.SetActive(true);
                Candy2.SetActive(true);
                Candy3.SetActive(false);
                Candy4.SetActive(false);
                Candy5.SetActive(false);
            }
            else if (score > 0)
            {
                Candy1.SetActive(true);
                Candy2.SetActive(false);
                Candy3.SetActive(false);
                Candy4.SetActive(false);
                Candy5.SetActive(false);
            }
            else
            {
                Candy1.SetActive(false);
                Candy2.SetActive(false);
                Candy3.SetActive(false);
                Candy4.SetActive(false);
                Candy5.SetActive(false);
            }
        }

        private void PlayAudioSource(AudioClip audio)
        {

            audioSource.clip = audio;
            audioSource.Play();


        }




    }
}
