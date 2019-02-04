using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class LinearMovement : MonoBehaviour
    {
        [SerializeField]
        private float MovementLength = 0f;

        [SerializeField]
        private float MovementDuration = 1f;

        [SerializeField]
        private bool OnAwake;

        private bool m_IsMoving;

        private Vector3 m_IdlePosition;

        private void Awake()
        {
            m_IdlePosition = transform.position;

            if (OnAwake)
                StartMoving();
        }

        private void OnDisable()
        {
            m_IsMoving = false;
        }

        public void StartMoving()
        {
            if (m_IsMoving)
                return;

            StartCoroutine(MoveAnimation());
        }

        public void StopMoving()
        {
            m_IsMoving = false;
        }

        IEnumerator MoveAnimation()
        {
            if (m_IsMoving)
                yield break;

            m_IsMoving = true;
            float currentDuration = MovementDuration / 2f;
            Vector3 startPosition = m_IdlePosition - MovementLength * transform.up;
            Vector3 endPosition = m_IdlePosition + MovementLength * transform.up;
            while (m_IsMoving)
            {
                if (currentDuration >= MovementDuration)
                {
                    var temp = startPosition;
                    startPosition = endPosition;
                    endPosition = temp;
                    transform.position = startPosition;
                    currentDuration = 0f;
                }
                transform.position = Vector3.Lerp(startPosition, endPosition, currentDuration / MovementDuration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            m_IsMoving = false;
        }
    }
}


