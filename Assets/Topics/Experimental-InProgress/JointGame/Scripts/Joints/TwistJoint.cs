using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class TwistJoint : Joint
    {
        protected override void StartMotion()
        {
            IsMoving = true;
        }

        protected override void UpdateMotion()
        {
            transform.Rotate(Vector3.up * 6f / MotionDuration, Space.Self); // 360 degrees / 60fps * MotionDuration = 6 / MotionDuration
        }
    }
}


