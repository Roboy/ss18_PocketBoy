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
            if (!m_IsRunning)
                return;

            //transform.Translate(Vector3.right * ForwardSpeed * Time.fixedDeltaTime, Space.Self);
            m_RigidBody.AddForce(Vector3.right * ForwardForce);
        }

        private void OnTriggerEnter(Collider other)
        {
            //ParabolaTrigger parabolaTrigger = null;
            //if ((parabolaTrigger = other.GetComponent<ParabolaTrigger>()) != null)
            //{
            //    var middleParabolaPoint = (transform.position + parabolaTrigger.ParabolaGoal) / 2f;
            //    middleParabolaPoint.y = Mathf.Max(transform.position.y, parabolaTrigger.ParabolaGoal.y) * 1.5f;
            //    m_ParabolaController.SetPoints(new Vector3[] { transform.position, middleParabolaPoint, parabolaTrigger.ParabolaGoal });
            //    m_ParabolaController.FollowParabola();
            //    m_RigidBody.isKinematic = true;
            //    m_IsJumping = true;
            //}

            TeleportTrigger teleportTrigger = null;
            if ((teleportTrigger = other.GetComponent<TeleportTrigger>()) != null)
            {
                transform.position = teleportTrigger.TeleportGoal;
                Debug.Log("asdasd");
            }
            Debug.Log("asdasd2");

            if (other.CompareTag("Deadzone"))
            {
                ResetPosition();
                Stop();
            }
        }
    }
}


