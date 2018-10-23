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

        private void Awake()
        {
            m_RigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            m_RigidBody.AddForce(Vector3.right * ForwardForce);
        }
    }
}


