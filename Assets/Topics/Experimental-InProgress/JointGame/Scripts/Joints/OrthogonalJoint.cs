using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class OrthogonalJoint : Joint
    {
        [SerializeField]
        private float MovementLength = 0f;

        protected override IEnumerator MotionAnimation()
        {
            if (m_IsMoving)
                yield break;

            m_IsMoving = true;
            float currentDuration = 0f;
            Vector3 startPosition = m_IdlePosition;
            Vector3 endPosition = m_IdlePosition - MovementLength * transform.up;
            while (m_IsMoving)
            {
                if (currentDuration >= MotionDuration)
                {
                    var temp = startPosition;
                    startPosition = endPosition;
                    endPosition = temp;
                    transform.position = startPosition;
                    currentDuration = 0f;
                }
                transform.position = Vector3.Lerp(startPosition, endPosition, currentDuration / MotionDuration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            m_IsMoving = false;
        }
    }
}