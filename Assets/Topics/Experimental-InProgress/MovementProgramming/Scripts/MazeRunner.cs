using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pocketboy.MovementProgramming
{
    public class MazeRunner : MonoBehaviour
    {

        public float MovementSpeed = 0.1f;
        public float TurningSpeed = 0.5f;
        [HideInInspector]
        public bool m_GoalHit = false;
        [HideInInspector]
        public bool m_DeadzoneHit = false;

        private bool m_PlayerMoving = false;
        private bool m_WallHit = false;
        
        private Pose m_InitPose;


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
        }

       
        public IEnumerator GoForward()
        {
            m_PlayerMoving = true;

            while (m_PlayerMoving)
            {

                if (m_WallHit || m_GoalHit || m_DeadzoneHit)
                    break;

                //If there is no wall, continue on moving forward
                transform.position += transform.forward * Time.deltaTime * MovementSpeed;
                yield return null;
            }

           
            m_PlayerMoving = false;
            yield return null;
        }


        public IEnumerator TurnAround(string direction)
        {
            m_PlayerMoving = true;

            float turnAngleY = 0.0f;
            float currentTurnAngleY = 0.0f;
            float initialAngleY = transform.localEulerAngles.y;
            float currentDuration = 0.0f;
            float Duration = TurningSpeed;

            if (direction == "Turn Left")
            {
                turnAngleY = initialAngleY - 90.0f;
            }
            if (direction == "Turn Right")
            {
                turnAngleY = initialAngleY + 90.0f;
            }

            while (currentDuration < Duration)
            {
                currentTurnAngleY = Mathf.Lerp(initialAngleY, turnAngleY, currentDuration / Duration);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentTurnAngleY, transform.localEulerAngles.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, turnAngleY, transform.localEulerAngles.z);

            m_PlayerMoving = false;
            yield return null;
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

