using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlatformPlayer : MonoBehaviour
    {
        [SerializeField]
        private Transform LevelsParent; 

        [SerializeField]
        private float ForwardForce = 1f;

        //[SerializeField] // will be used later? when real roboy model arrives
        //private float ForwardSpeed = 10f;

        private Rigidbody m_RigidBody;

        private int m_CurrentPlatform = -1;

        private bool m_IsRunning = false;

        private int m_TeleportsLeft = 0;

        private void Awake()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_RigidBody.isKinematic = true;

            PitchPlatformerEvents.PlatformFinishedEvent += GoToNextPlatform;
            PitchPlatformerEvents.ReachedGoalEvent += Reset;
            PitchPlatformerEvents.ShowLevelEvent += ResetPosition;
        }

        private void FixedUpdate()
        {
            if (!m_IsRunning || m_TeleportsLeft <= 0 && m_CurrentPlatform != PitchPlatformerManager.Instance.GoalIndexInCurrentLevel)
            {
                m_RigidBody.isKinematic = true;
                return;
            }

            m_RigidBody.AddForce(LevelsParent.right * ForwardForce);
        }

        private void OnTriggerEnter(Collider other)
        {
            TeleportTrigger teleportTrigger = null;
            if ((teleportTrigger = other.GetComponent<TeleportTrigger>()) != null)
            {
                if (teleportTrigger.ShouldTeleportInstantly)
                {
                    transform.position = teleportTrigger.TeleportGoal;
                }
                else
                {
                    StartCoroutine(TeleportAnimation(teleportTrigger.TeleportGoal));
                }               
                m_CurrentPlatform++;
                m_TeleportsLeft--;
            }

            if (other.CompareTag("Deadzone"))
            {
                ResetPosition();
            }
        }

        public void ResetPosition()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void GoToNextPlatform()
        {
            m_TeleportsLeft++;

            if(!m_IsRunning) // still in spawn position
            {
                m_IsRunning = true;                
            }
            m_RigidBody.isKinematic = false;
        }

        private void Reset()
        {
            m_CurrentPlatform = -1;
            m_IsRunning = false;
            m_RigidBody.isKinematic = true;
        }

        private IEnumerator TeleportAnimation(Vector3 goalPosition)
        {
            m_RigidBody.isKinematic = true;
            float duration = 0.1f;
            float currentDuration = 0f;
            Vector3 initPosition = transform.position;
            while (currentDuration < duration)
            {
                transform.position = Vector3.Lerp(initPosition, goalPosition, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            transform.position = goalPosition;
            m_RigidBody.isKinematic = false;
        }
    }
}


