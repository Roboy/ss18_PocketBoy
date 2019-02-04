using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class LinearJoint : Joint
    {
        [SerializeField]
        private float MovementLength = 0f;

        private float m_CurrentTimer = 0f;

        private Vector3 m_CurrentPositon;

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
            m_CurrentPositon = transform.localPosition;
            m_CurrentPositon.y = Mathf.PingPong(m_CurrentTimer / (MotionDuration * 2f), MovementLength) + m_IdlePosition.y;
            transform.localPosition = m_CurrentPositon;
        }
    }
}


