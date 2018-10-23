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

        private void FixedUpdate()
        {
            if (!m_IsRunning)
                return;

            m_RigidBody.AddForce(Vector3.right * ForwardForce);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Deadzone"))
            {
                transform.position = m_CurrentSpawnPoint;
                Stop();
            }
        }
    }
}


