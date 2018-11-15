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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, (transform.lossyScale.z / 2.0f) * 1.1f);
        }
        public IEnumerator GoForward()
        {
            m_PlayerMoving = true;
            RaycastHit hit = new RaycastHit();
            int layermask = LayerMask.GetMask("MazeObjects");
            Debug.Log(layermask);

            while (m_PlayerMoving)
            {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, (transform.lossyScale.z / 2.0f) * 1.1f, transform.forward, (transform.lossyScale.z / 2.0f) * 0.1f, layermask);
                string output = "";
                foreach (RaycastHit h in hits)
                {
                    output += " // ";
                    output += h.collider.name;
                }
                Debug.Log(output);

                if (Physics.SphereCast(transform.position, (transform.lossyScale.z / 2.0f) * 1.1f, transform.forward, out hit, (transform.lossyScale.z / 2.0f) * 0.1f, layermask))
                {
                    Debug.Log("Spherecasting!");
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
                        CodeManager.Instance.StopExecution();
                        break;
                    }
                    else if (hit.transform.tag == "WinningZone")
                    {
                        break;
                    }


                }

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

        public void SetInitPose()
        {
            m_InitPose = new Pose(transform.position, transform.rotation);
        }
    }
}

