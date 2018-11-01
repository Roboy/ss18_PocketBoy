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

        private int m_LastBuiltPlatform = -1;

        private bool m_IsRunning = false;

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
            Debug.Log(m_CurrentPlatform + " : " + PitchPlatformerManager.Instance.CurrentPlatformInCurrentLevel + " : " + PitchPlatformerManager.Instance.GoalIndexInCurrentLevel);
            if (!m_IsRunning || (m_CurrentPlatform > -1 && m_CurrentPlatform == m_LastBuiltPlatform - 1 
                && m_CurrentPlatform < PitchPlatformerManager.Instance.GoalIndexInCurrentLevel ))
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
                transform.position = teleportTrigger.TeleportGoal;
                m_CurrentPlatform++;
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
            if(!m_IsRunning) // still in spawn position
            {
                m_IsRunning = true;
                m_RigidBody.isKinematic = false;
            }
            
            m_LastBuiltPlatform++;
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
            float duration = 0.5f;
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


