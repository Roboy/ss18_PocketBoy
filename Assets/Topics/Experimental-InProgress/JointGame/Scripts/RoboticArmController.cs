using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class RoboticArmController : MonoBehaviour
    {
        [SerializeField]
        private Transform Effector;

        [SerializeField]
        private Joint MotionJoint;

        [SerializeField]
        private bool OnAwake;

        private Vector3 m_EffectorIdlePosition;

        private Quaternion m_EffectorIdleRotation;

        private void Awake()
        {
            m_EffectorIdlePosition = Effector.transform.localPosition;
            m_EffectorIdleRotation = Effector.transform.localRotation;

            if (OnAwake)
                StartMotion();
        }

        public void StartMotion(Joint joint = null)
        {
            if (MotionJoint != null)
                StopMotion();

            if (joint)
                MotionJoint = joint;

            if (MotionJoint == null)
                return;

            MotionJoint.gameObject.SetActive(true);
            MotionJoint.ApplyMotion(Effector);
        }

        public void StopMotion()
        {
            if (MotionJoint != null)
                MotionJoint.StopMotion();

            MotionJoint.gameObject.SetActive(false);
            ResetEffector();
        }

        private void ResetEffector()
        {
            Effector.SetParent(transform);
            Effector.transform.localPosition = m_EffectorIdlePosition;
            Effector.transform.localRotation = m_EffectorIdleRotation;
        }
    }
}