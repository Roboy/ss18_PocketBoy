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

        [SerializeField]
        private Transform LevelsParent;

        private List<JointGameLevel> m_Levels = new List<JointGameLevel>();

        /// <summary>
        /// Joint chosen by the user.
        /// </summary>
        private Joint m_CurrentUserJoint;

        private int m_CurrentLevelIndex = 0;

        private void Awake()
        {
            AddSubscribers();
            SetupLevels();

            if (LevelManager.InstanceExists)
            {
                LevelManager.Instance.RegisterGameObjectWithRoboy(LevelsParent.gameObject, Vector3.right * 0.2f + Vector3.forward * 0.3f, Quaternion.identity);
                LevelsParent.forward = -LevelsParent.forward;
            }
        }

        private void Start()
        {
            StartGame();
        }

        private void AddSubscribers()
        {
            LinearJointButton.onClick.AddListener( () => AddJoint(LinearJoint));
            OrthogonalJointButton.onClick.AddListener(() => AddJoint(OrthogonalJoint));
            RotationalJointButton.onClick.AddListener(() => AddJoint(RotationalJoint));
            TwistJointButton.onClick.AddListener(() => AddJoint(TwistJoint));
            RevolvingJointButton.onClick.AddListener(() => AddJoint(RevolvingJoint));
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

            if (m_Levels[m_CurrentLevelIndex].CheckJoint(m_CurrentUserJoint))
            {
                ShowNextLevel();
            }
            else
            {
                Handheld.Vibrate();
            }
        }

        private void SetupLevels()
        {
            foreach (Transform child in LevelsParent)
            {
                JointGameLevel level = null;
                if ((level = child.GetComponent<JointGameLevel>()) != null)
                {
                    m_Levels.Add(level);
                }
            }
        }

        private void StartGame()
        {
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = 0;
            m_Levels.Shuffle();
            m_Levels[0].Show();
        }

        private void ShowLevel(int index)
        {
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = MathUtility.WrapArrayIndex(index, m_Levels.Count); ;
            m_Levels[m_CurrentLevelIndex].Show();
        }

        private void ShowNextLevel()
        {
            if (m_CurrentLevelIndex == m_Levels.Count - 1)
            {
                StartGame();
                return;
            }
            
            ShowLevel(m_CurrentLevelIndex + 1);
        }
    }
}
