using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class RevolvingJoint : Joint
    {
        protected override IEnumerator MotionAnimation()
        {
            if (m_IsMoving)
                yield break;

            m_IsMoving = true;

            while (m_IsMoving)
            {
                transform.Rotate(Vector3.up * 6f / MotionDuration, Space.Self); // 360 degrees / 60fps * MotionDuration = 6 / MotionDuration
                yield return null;
            }
            m_IsMoving = false;
        }
    }
}
