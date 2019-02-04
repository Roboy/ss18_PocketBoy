using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class RotationalJoint : Joint
    {
        [SerializeField]
        private float RotationLimit = 0f;

        [SerializeField]
        private Transform MovingLink;

        private Vector3 m_EulerAngles;

        private float m_CurrentTimer;

        private void Start()
        {
            RotationLimit = Mathf.Clamp(RotationLimit, -89f, 89f); // HACK TO AVOID ROTATION BUGS WITH HIGHER ANGLES THAN 90 DEGREES        
        }

        protected override void StartMotion()
        {
            if (IsMoving)
                return;

            m_CurrentTimer = 0f;
            IsMoving = true;
        }

        protected override void UpdateMotion()
        {
            m_CurrentTimer += Time.fixedDeltaTime;
            m_EulerAngles.z = Mathf.PingPong(m_CurrentTimer / (MotionDuration * 2f), 2 * RotationLimit) - RotationLimit;
            MovingLink.localEulerAngles = m_EulerAngles;
        }
    }
}
