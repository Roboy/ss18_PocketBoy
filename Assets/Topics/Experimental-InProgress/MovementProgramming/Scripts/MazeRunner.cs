using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pocketboy.MovementProgramming
{
    public class MazeRunner : MonoBehaviour
    {

        public float MovementSpeed = 0.1f;
        public float TurningDuration = 0.5f;
        [HideInInspector]
        public bool m_GoalHit = false;
        [HideInInspector]
        public bool m_DeadzoneHit = false;

        public bool ExecutingAction { get; private set; }

        private bool m_PlayerMoving = false;
        private bool m_PlayerTurning = false;
        private bool m_WallHit = false;
        
        private Pose m_InitPose;

        private float m_InitAngle = 0f;
        private float m_TargetAngle = 0f;

        private float m_CurrentDuration;


        private void Start()
        {
            SetInitPose();
        }

        public bool IsMoving()
        {
            return m_PlayerMoving;
        }

        public void ResetPlayerPose()
        {
            transform.position = m_InitPose.position;
            transform.rotation = m_InitPose.rotation;

            m_PlayerTurning = false;
            m_PlayerMoving = false;
            ExecutingAction = false;
        }

        private void Update()
        {
            if (m_PlayerMoving)
            {
                GoForwardInternal();
            }              
            else if(m_PlayerTurning)
            {
                TurnAroundInternal();
            }
        }

        public void GoForward()
        {
            ExecutingAction = true;
            m_PlayerMoving = true;
        }

        public void TurnAround(string direction)
        {
            ExecutingAction = true;
            m_PlayerTurning = true;
            m_InitAngle = transform.localEulerAngles.y;
            m_CurrentDuration = 0f;

            if (direction == "Turn Left")
            {
                m_TargetAngle = m_InitAngle - 90.0f;
            }
            if (direction == "Turn Right")
            {
                m_TargetAngle = m_InitAngle + 90.0f;
            }         
        }

        private void GoForwardInternal()
        {
            if (!m_PlayerMoving)
                return;

            if (m_WallHit || m_GoalHit || m_DeadzoneHit)
            {
                m_PlayerMoving = false;
                ExecutingAction = false;
                CodeManager.Instance.NextInstruction();
                return;
            }
            transform.position += transform.forward * Time.deltaTime * MovementSpeed;
        }

        public void TurnAroundInternal()
        {
            if (!m_PlayerTurning)
                return;

            if (m_CurrentDuration < TurningDuration)
            {
                var currentAngle = Mathf.Lerp(m_InitAngle, m_TargetAngle, m_CurrentDuration / TurningDuration);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);
                m_CurrentDuration += Time.deltaTime;
            }
            else
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, m_TargetAngle, transform.localEulerAngles.z);
                m_PlayerTurning = false;
                ExecutingAction = false;
                CodeManager.Instance.NextInstruction();
            }
        }

        public void SetInitPose()
        {
            m_InitPose = new Pose(transform.position, transform.rotation);
        }


        private void OnCollisionEnter(Collision collision)
        {

            if (collision.transform.tag == "Wall")
            {
                m_WallHit = true;
            }

            if (collision.transform.tag == "WinningZone")
            {
                m_GoalHit = true;
            }

            if (collision.transform.tag == "Deadzone")
            {
                m_DeadzoneHit = true;
            }
        }

        private void OnCollisionStay(Collision collision)
        {

            if (collision.transform.tag == "Wall")
            {
                m_WallHit = true;
            }

            if (collision.transform.tag == "WinningZone")
            {
                m_GoalHit = true;
            }

            if (collision.transform.tag == "Deadzone")
            {
                m_DeadzoneHit = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.transform.tag == "Wall")
            {
                m_WallHit = false;
            }

            if (collision.transform.tag == "WinningZone")
            {
                m_GoalHit = false;
            }

            if (collision.transform.tag == "Deadzone")
            {
                m_DeadzoneHit = false;
            }
        }
    }
}

