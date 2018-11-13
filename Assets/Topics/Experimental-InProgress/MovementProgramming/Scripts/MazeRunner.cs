using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pocketboy.MovementProgramming
{
    public class MazeRunner : MonoBehaviour
    {

        public float MovementSpeed = 0.1f;
        public float TurningSpeed = 0.5f;

        private bool m_PlayerMoving = false;
        private bool m_WallHit = false;
        private Pose m_InitPose;

        private void Start()
        {
            m_InitPose = new Pose(transform.localPosition, transform.localRotation);
        }

        public bool IsMoving()
        {
            return m_PlayerMoving;
        }

        public void ResetPlayerPose()
        {
            transform.localPosition = m_InitPose.position;
            transform.localRotation = m_InitPose.rotation;
        }


        public IEnumerator GoForward()
        {
            m_PlayerMoving = true;
            while (m_PlayerMoving)
            {
                RaycastHit hit = new RaycastHit();
                if (Physics.SphereCast(transform.localPosition, transform.localScale.z / 2.0f, transform.forward, out hit, transform.localScale.z * 0.1f))
                {
                    string tag = hit.transform.tag;
                    if (tag == null)
                    {
                        continue;
                    }
                    else if (hit.transform.tag == "Wall")
                    {
                        
                        break;
                    }
                    else if (hit.transform.tag == "Deadzone")
                    {
                        CodeManager.Instance.StopExecution(false);
                        break;
                    }
                    else if (hit.transform.tag == "WinningZone")
                    {
                        CodeManager.Instance.StopExecution(true);
                        break;
                    }


                }

                //If there is no wall, continue on moving forward
                transform.localPosition += transform.forward.normalized * Time.deltaTime * MovementSpeed;
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

            if (direction == "Left")
            {
                turnAngleY = initialAngleY - 90.0f;
            }
            if (direction == "Right")
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
    }
}

