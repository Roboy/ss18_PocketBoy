using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlatformPlayer : MonoBehaviour
    {
        [SerializeField]
        private float ForwardForce = 1f;

        [SerializeField]
        private float ForwardSpeed = 10f;

        private Rigidbody m_RigidBody;

        private bool m_IsRunning;

        private Vector3 m_CurrentSpawnPoint;

        private void Awake()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_RigidBody.isKinematic = true;
        }
        

        public void Go()
        {
            m_RigidBody.isKinematic = false;
            m_IsRunning = true;
        }

        public void Stop()
        {
            m_RigidBody.isKinematic = true;
            m_IsRunning = false;
        }

        public void SetSpawnPosition(Vector3 position)
        {
            m_CurrentSpawnPoint = position;
        }

        public void ResetPosition()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void FixedUpdate()
        {
            if (!m_IsRunning && !m_RigidBody.isKinematic)
                return;

            //transform.Translate(Vector3.right * ForwardSpeed * Time.fixedDeltaTime, Space.Self);
            m_RigidBody.AddForce(Vector3.right * ForwardForce);
        }

        private void OnTriggerEnter(Collider other)
        {
            TeleportTrigger teleportTrigger = null;
            if ((teleportTrigger = other.GetComponent<TeleportTrigger>()) != null)
            {
                StartCoroutine(TeleportAnimation(teleportTrigger.TeleportGoal));
            }

            if (other.CompareTag("Deadzone"))
            {
                ResetPosition();
                Stop();
            }
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


