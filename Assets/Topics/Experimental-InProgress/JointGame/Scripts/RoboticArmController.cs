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
        private LinearJoint LinearJointComponent;

        [SerializeField]
        private OrthogonalJoint OrthogonalJointComponent;

        [SerializeField]
        private RotationalJoint RotationalJointComponent;

        [SerializeField]
        private TwistJoint TwistJointComponent;

        [SerializeField]
        private RevolvingJoint RevolvingJointComponent;

        private Vector3 m_EffectorIdlePosition;

        private Quaternion m_EffectorIdleRotation;

        private Joint m_CurrentJoint;


        private void Awake()
        {
            m_EffectorIdlePosition = Effector.transform.localPosition;
            m_EffectorIdleRotation = Effector.transform.localRotation;
        }

        public void StopMotion(Joint newMotionJoint = null)
        {
            if (m_CurrentJoint != null)
            {
                m_CurrentJoint.StopMotion();
                if (newMotionJoint != null && m_CurrentJoint.gameObject != newMotionJoint.gameObject)
                {
                    m_CurrentJoint.gameObject.SetActive(false);
                    newMotionJoint.gameObject.SetActive(true);
                }
                m_CurrentJoint = null;
            }
            
            ResetEffector();
        }

        public void LinearMotion()
        {
            JointMotion(LinearJointComponent);
        }

        public void OrthogonalMotion()
        {
            JointMotion(OrthogonalJointComponent);
        }

        public void RotationalMotion()
        {
            JointMotion(RotationalJointComponent);
        }

        public void TwistMoting()
        {
            JointMotion(TwistJointComponent);
        }

        public void RevolvingMotion()
        {
            JointMotion(RevolvingJointComponent);
        }

        private void JointMotion(Joint joint)
        {
            if (joint == null)
                return;

            if (m_CurrentJoint != null)
                StopMotion();

            m_CurrentJoint = joint;
            m_CurrentJoint.ApplyMotion(Effector);
        }

        private void ResetEffector()
        {
            Effector.SetParent(transform);
            Effector.transform.localPosition = m_EffectorIdlePosition;
            Effector.transform.localRotation = m_EffectorIdleRotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckIfCollisionIsPoint(other);
        }

        private void OnTriggerStay(Collider other)
        {
            CheckIfCollisionIsPoint(other);
        }

        private void CheckIfCollisionIsPoint(Collider other)
        {
            if (m_CurrentJoint != null && m_CurrentJoint.IsMoving && other.GetComponent<JointGamePoint>() != null)
            {
                other.gameObject.SetActive(false);
                JointGameManager.Instance.PointWasHit();
            }
        }
    }
}