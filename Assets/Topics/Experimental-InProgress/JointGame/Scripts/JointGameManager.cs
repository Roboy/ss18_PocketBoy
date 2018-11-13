using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;

namespace Pocketboy.JointGame
{
    public class JointGameManager : Singleton<JointGameManager>
    {
        [Header("UI Stuff")]

        [SerializeField]
        private Button LinearJointButton;

        [SerializeField]
        private Button OrthogonalJointButton;

        [SerializeField]
        private Button RotationalJointButton;

        [SerializeField]
        private Button TwistJointButton;

        [SerializeField]
        private Button RevolvingJointButton;

        [Header("Effector and Joints")]

        [SerializeField]
        private Transform EndEffector;

        [SerializeField]
        private Joint LinearJoint;

        [SerializeField]
        private Joint OrthogonalJoint;

        [SerializeField]
        private Joint RotationalJoint;

        [SerializeField]
        private Joint TwistJoint;

        [SerializeField]
        private Joint RevolvingJoint;

        [Header("Arrows")]

        [SerializeField]
        private Transform LinearMotionArrow;

        [SerializeField]
        private Transform OrthogonalMotionArrow;

        [SerializeField]
        private Transform RotationalMotionArrow;

        [SerializeField]
        private Transform TwistMotionArrow;

        [SerializeField]
        private Transform RevolvingMotionArrow;

        /// <summary>
        /// Joint chosen by the user.
        /// </summary>
        private Joint m_CurrentUserJoint;

        /// <summary>
        /// Correct joint of the current level.
        /// </summary>
        private Joint m_CurrentLevelJoint;

        private List<Transform> m_Levels = new List<Transform>();

        private void Awake()
        {
            AddSubscribers();
        }

        private void AddSubscribers()
        {
            LinearJointButton.onClick.AddListener(AddLinearJoint);
            OrthogonalJointButton.onClick.AddListener(AddOrthogonalJoint);
            RotationalJointButton.onClick.AddListener(AddRotationalJoint);
            TwistJointButton.onClick.AddListener(AddTwistJoint);
            RevolvingJointButton.onClick.AddListener(AddRevolvingJoint);
        }

        private void AddLinearJoint()
        {
            if (m_CurrentUserJoint == LinearJoint)
                return;

            AddJoint(LinearJoint);
        }

        private void AddOrthogonalJoint()
        {
            if (m_CurrentUserJoint == OrthogonalJoint)
                return;

            AddJoint(OrthogonalJoint);
        }

        private void AddRotationalJoint()
        {
            if (m_CurrentUserJoint == RotationalJoint)
                return;

            AddJoint(RotationalJoint);
        }

        private void AddTwistJoint()
        {
            if (m_CurrentUserJoint == TwistJoint)
                return;

            AddJoint(TwistJoint);
        }

        private void AddRevolvingJoint()
        {
            if (m_CurrentUserJoint == RevolvingJoint)
                return;

            AddJoint(RevolvingJoint);
        }

        private void AddJoint(Joint joint)
        {
            if (m_CurrentUserJoint != null)
            {
                m_CurrentUserJoint.StopMotion();
                m_CurrentUserJoint.gameObject.SetActive(false);
            }

            joint.gameObject.SetActive(true);
            joint.ApplyMotion(EndEffector);

            m_CurrentUserJoint = joint;
        }

        private void SetupLevels()
        {

        }
    }
}
