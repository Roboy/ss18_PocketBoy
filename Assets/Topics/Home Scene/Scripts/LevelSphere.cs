using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class LevelSphere : MonoBehaviour
    {
        public string SceneToLoad { get { return SceneName; } }

        [SerializeField]
        private string SceneName;

        [SerializeField, Range(0f, 1000f)]
        private float MaxRotationSpeed = 100f;

        [SerializeField, Range(0f, 100f)]
        private float MinRotationSpeed = 10f;

        private float m_CurrentRotationSpeed = 0f;

        private float m_InitDistance;

        private float m_CurrentDistance = 0f;

        private float m_Speed;

        private void Start()
        {
            m_CurrentRotationSpeed = MinRotationSpeed;
        }

        private void FixedUpdate()
        {
            transform.Rotate(Vector3.up, m_CurrentRotationSpeed * Time.fixedDeltaTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            m_InitDistance = (other.transform.position - transform.position).magnitude;
        }

        private void OnTriggerExit(Collider other)
        {
            m_CurrentDistance = 0f;
            m_InitDistance = 0f;
            m_CurrentRotationSpeed = MinRotationSpeed;
        }

        private void OnTriggerStay(Collider other)
        {
            m_CurrentDistance = Mathf.Max(0f, (other.transform.position - transform.position).magnitude - 0.5f * transform.localScale.x);
            m_CurrentRotationSpeed = Mathf.Lerp(0f, MaxRotationSpeed, (1f - m_CurrentDistance / m_InitDistance));
        }
    }
}


