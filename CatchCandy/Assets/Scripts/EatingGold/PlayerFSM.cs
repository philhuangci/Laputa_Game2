using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EatingGold
{
    public class PlayerFSM : SuperStateMachine
    {
        public enum PlayerState
        {
            Idle,
            Start,
            Selected,
            WaitingOthers,
            Jump,
            Win,
            Defeat,
            Rotate,
            FallDown,
        }

        Animator Animator;

        CharacterController CharacterController;

        public enum Selection
        {
            Left,
            Forward,
            Right,
            None
        }

        public enum PlayerDirection
        {
            North,
            East,
            South,
            West
        }

        public SceneControl SceneControl;

        public PlayerDirection PlayerDir;

        public ParticleSystem HitEffect;

        public ParticleSystem AshEffect;

        public bool IsAI;

        Selection PlayerSel;

        float RotateAngle;
        float RotateSpeed = 800;

        float MoveSpeed = 5;
        Vector3 JumpDirection;
        Vector3 Dest;

        // Start is called before the first frame update
        void Start()
        {
            Animator = GetComponent<Animator>();
            CharacterController = GetComponent<CharacterController>();
            currentState = PlayerState.Idle;

            transform.rotation = Quaternion.Euler(0, GetDefaultRotation(), 0);
            transform.position = GetDefaultPostion();
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

        }

        void Start_EnterState()
        {
            Animator.Play("ready");
        }

        void Start_ExitState()
        {

        }

        void Start_SuperUpdate()
        {
            if (IsAI && Time.time - timeEnteredState >= 10)
            {
                return;
            }

            if (PlayerDir == PlayerDirection.North)
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    PlayerSel = Selection.Forward;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PlayerSel = Selection.Right;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    PlayerSel = Selection.Left;
                    currentState = PlayerState.Selected;
                }
            }
            else if (PlayerDir == PlayerDirection.East)
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    PlayerSel = Selection.Left;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PlayerSel = Selection.Forward;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    PlayerSel = Selection.Right;
                    currentState = PlayerState.Selected;
                }
            }
            else if (PlayerDir == PlayerDirection.South)
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    PlayerSel = Selection.Right;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PlayerSel = Selection.Left;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    PlayerSel = Selection.Forward;
                    currentState = PlayerState.Selected;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    PlayerSel = Selection.Right;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    PlayerSel = Selection.Forward;
                    currentState = PlayerState.Selected;
                }
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    PlayerSel = Selection.Left;
                    currentState = PlayerState.Selected;
                }
            }
        }

        void Selected_EnterState()
        {
            Animator.Play("ok");
        }

        void Selected_ExitState()
        {

        }

        void Selected_SuperUpdate()
        {
            var animatorState = Animator.GetCurrentAnimatorStateInfo(0);
            if (animatorState.IsName("ok"))
            {
                float length = animatorState.length;

                if (Time.time - timeEnteredState >= length)
                {
                    currentState = PlayerState.WaitingOthers;
                }
            }
        }

        void WaitingOthers_EnterState()
        {
            Animator.Play("idle");
        }

        void WaitingOthers_ExitState()
        {

        }

        void WaitingOthers_SuperUpdate()
        {

        }

        void Jump_EnterState()
        {
            Animator.Play("Jump");
        }

        void Jump_ExitState()
        {

        }

        void Jump_SuperUpdate()
        {
            if ((transform.position - Dest).magnitude > MoveSpeed * Time.fixedDeltaTime * 2)
            {
                CharacterController.Move(JumpDirection * MoveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                transform.position = Dest;
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

        void Rotate_EnterState()
        {
            Animator.Play("walk");
        }

        void Rotate_ExitState()
        {

        }

        void Rotate_SuperUpdate()
        {
            if (Mathf.Abs(transform.rotation.eulerAngles.y - RotateAngle) > 1f)
            {
                float angle = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.y, RotateAngle, RotateSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        void FallDown_EnterState()
        {
            Animator.Play("FallDown");
        }

        void FallDown_ExitState()
        {

        }

        void FallDown_SuperUpdate()
        {
            if ((transform.position - Dest).magnitude > MoveSpeed * Time.fixedDeltaTime * 2)
            {
                CharacterController.Move(JumpDirection * MoveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                transform.position = Dest;
            }
        }
        #endregion
        //=========================================================
        public void Idle()
        {
            currentState = PlayerState.Idle;
        }

        public void StartSelecting()
        {
            PlayerSel = Selection.None;
            currentState = PlayerState.Start;
        }

        public void Select(Selection sel)
        {
            PlayerSel = sel;
            currentState = PlayerState.Selected;
        }

        public void EndSelect()
        {
            currentState = PlayerState.Idle;
        }

        public int GetSelectedCloud()
        {
            if (PlayerDir == PlayerDirection.North)
            {
                if (PlayerSel == Selection.Forward)
                {
                    return 4;
                }
                else if (PlayerSel == Selection.Left)
                {
                    return 2;
                }
                else if (PlayerSel == Selection.Right)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (PlayerDir == PlayerDirection.South)
            {
                if (PlayerSel == Selection.Forward)
                {
                    return 4;
                }
                else if (PlayerSel == Selection.Left)
                {
                    return 6;
                }
                else if (PlayerSel == Selection.Right)
                {
                    return 8;
                }
                else
                {
                    return 7;
                }
            }
            else if (PlayerDir == PlayerDirection.West)
            {
                if (PlayerSel == Selection.Forward)
                {
                    return 4;
                }
                else if (PlayerSel == Selection.Left)
                {
                    return 0;
                }
                else if (PlayerSel == Selection.Right)
                {
                    return 6;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
                if (PlayerSel == Selection.Forward)
                {
                    return 4;
                }
                else if (PlayerSel == Selection.Left)
                {
                    return 8;
                }
                else if (PlayerSel == Selection.Right)
                {
                    return 2;
                }
                else
                {
                    return 5;
                }
            }
        }

        public void RotateToSel()
        {
            if (PlayerSel == Selection.Left)
            {
                RotateAngle = transform.rotation.eulerAngles.y - 90;
                currentState = PlayerState.Rotate;

            }
            else if (PlayerSel == Selection.Right)
            {
                RotateAngle = transform.rotation.eulerAngles.y + 90;
                currentState = PlayerState.Rotate;
            }

        }

        public void RotateBack()
        {
            RotateAngle = transform.rotation.eulerAngles.y + 180;
            currentState = PlayerState.Rotate;
        }

        public void RotateToDefault()
        {

            //float angleY = transform.rotation.eulerAngles.y;
            //Mathf. 
            if (PlayerDir == PlayerDirection.East && Mathf.Abs(transform.rotation.eulerAngles.y + 90) % 360 > 1f)
            {
                RotateAngle = -90;
                currentState = PlayerState.Rotate;
            }
            else if (PlayerDir == PlayerDirection.West && Mathf.Abs(transform.rotation.eulerAngles.y - 90) % 360 > 1f)
            {
                RotateAngle = 90;
                currentState = PlayerState.Rotate;
            }
            else if (PlayerDir == PlayerDirection.North && Mathf.Abs(transform.rotation.eulerAngles.y - 180) % 360 > 1f)
            {
                RotateAngle = 180;
                currentState = PlayerState.Rotate;
            }
            else if (PlayerDir == PlayerDirection.South && Mathf.Abs(transform.rotation.eulerAngles.y) % 360 > 1f)
            {
                RotateAngle = 0;
                currentState = PlayerState.Rotate;
            }
        }

        public void JumpToSel()
        {
            if (PlayerSel != Selection.None)
            {
                Dest = SceneControl.Clouds[GetSelectedCloud()].transform.position;
                JumpDirection = Vector3.Normalize(Dest - transform.position);
                currentState = PlayerState.Jump;
            }
        }

        public void ShowResult(int result)
        {
            if (result > 0)
            {
                currentState = PlayerState.Win;
            }
            else
            {
                currentState = PlayerState.Defeat;
            }
        }

        public void JumpBack()
        {
            Dest = GetDefaultPostion();
            JumpDirection = Vector3.Normalize(Dest - transform.position);
            currentState = PlayerState.Jump;
        }


        bool HasHit = false;
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.tag != "Terrain")
            {
                Dest = GetDefaultPostion();
                JumpDirection = Vector3.Normalize(Dest - transform.position);
                currentState = PlayerState.FallDown;

                if (HitEffect)
                {
                    HitEffect.transform.position = hit.point;
                    HitEffect.Play();
                }
                HasHit = true;
            }
            else if (currentState.Equals(PlayerState.FallDown))
            {
                if (AshEffect && HasHit)
                {
                    AshEffect.transform.position = hit.point;
                    AshEffect.Play();
                    HasHit = false;
                }
            }
        }

        Vector3 GetDefaultPostion()
        {
            Vector3 dest;
            if (PlayerDir == PlayerDirection.North)
            {
                dest = SceneControl.Clouds[1].transform.position;
            }
            else if (PlayerDir == PlayerDirection.East)
            {
                dest = SceneControl.Clouds[5].transform.position;
            }
            else if (PlayerDir == PlayerDirection.South)
            {
                dest = SceneControl.Clouds[7].transform.position;
            }
            else
            {
                dest = SceneControl.Clouds[3].transform.position;
            }
            return dest;
        }

        float GetDefaultRotation()
        {
            if (PlayerDir == PlayerDirection.East)
            {
                return -90;
            }
            else if (PlayerDir == PlayerDirection.West)
            {
                return 90;
            }
            else if (PlayerDir == PlayerDirection.North)
            {
                return 180;
            }
            else
            {
                return 0;
            }
        }
    }


}
