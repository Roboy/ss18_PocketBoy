using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class RotationalJoint : Joint
    {
        [SerializeField]
        private float RotationLimit = 0f;

        private void Start()
        {
            RotationLimit = Mathf.Clamp(RotationLimit, -89f, 89f); // HACK TO AVOID ROTATION BUGS WITH HIGHER ANGLES THAN 90 DEGREES        
        }

        protected override IEnumerator MotionAnimation()
        {
            if (m_IsMoving)
                yield break;

            m_IsMoving = true;
            float currentDuration = MotionDuration / 2f;
            Quaternion startRotation = transform.rotation * Quaternion.AngleAxis(-RotationLimit, Vector3.forward);
            Quaternion endRotation = transform.rotation * Quaternion.AngleAxis(RotationLimit, Vector3.forward);
            while (m_IsMoving)
            {
                if (currentDuration >= MotionDuration)
                {
                    var temp = startRotation;
                    startRotation = endRotation;
                    endRotation = temp;
                    transform.rotation = startRotation;
                    currentDuration = 0f;
                }
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, currentDuration / MotionDuration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            m_IsMoving = false;
        }
    }
}
