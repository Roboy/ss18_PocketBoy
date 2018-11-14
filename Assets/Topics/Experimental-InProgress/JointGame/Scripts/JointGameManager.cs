using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;
using TMPro;

namespace Pocketboy.JointGame
{
    public class JointGameManager : Singleton<JointGameManager>
    {
        [Header("Game Settings")]

        [SerializeField]
        private float TimePerGame;

        [SerializeField]
        private int MaxPointsPerRound;

        [Header("UI Joint Buttons")]

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

        [Header("UI Feedback")]

        [SerializeField]
        private GameObject FeedbackParent;

        [SerializeField]
        private TextMeshProUGUI PointsText;

        [SerializeField]
        private TextMeshProUGUI TimeText;

        [Header("Effector and Joints")]

        [SerializeField]
        private RoboticArmController UserRoboticArm;

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

        [Header("Other stuff")]

        [SerializeField]
        private Transform LevelsParent;

        [SerializeField]
        private GameObject TryingInstruction;

        [SerializeField]
        private GameObject GameInstruction;

        private List<JointGameLevel> m_Levels = new List<JointGameLevel>();

        /// <summary>
        /// Joint chosen by the user.
        /// </summary>
        private Joint m_CurrentUserJoint;

        private int m_CurrentLevelIndex = 0;

        private float m_CurrentTime = 0;

        private float m_CurrentPoints = 0;

        private int m_CurrentTry = 1;

        private bool m_IsGameStarted;

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

        private void Update()
        {
            if (!m_IsGameStarted)
                return;

            m_CurrentTime -= Time.deltaTime;
            TimeText.text = m_CurrentTime.ToString("n1");

            if (m_CurrentTime < 0f)
            {
                StopGame();
            }

            PointsText.text = m_CurrentPoints.ToString("n0");
        }

        private void AddSubscribers()
        {
            LinearJointButton.onClick.AddListener( () => ChangeJoint(LinearJoint));
            OrthogonalJointButton.onClick.AddListener(() => ChangeJoint(OrthogonalJoint));
            RotationalJointButton.onClick.AddListener(() => ChangeJoint(RotationalJoint));
            TwistJointButton.onClick.AddListener(() => ChangeJoint(TwistJoint));
            RevolvingJointButton.onClick.AddListener(() => ChangeJoint(RevolvingJoint));
        }

        private void ChangeJoint(Joint joint)
        {           
            if (!m_IsGameStarted)
            {
                if (m_CurrentUserJoint != null)
                {
                    m_CurrentUserJoint.StopMotion();
                    m_CurrentUserJoint.gameObject.SetActive(false);
                }

                joint.gameObject.SetActive(true);
                joint.ApplyMotion(EndEffector);
            }
            else
            {
                if (m_Levels[m_CurrentLevelIndex].CheckJoint(joint))
                {
                    m_CurrentPoints += MaxPointsPerRound / (float)m_CurrentTry;
                    ShowNextLevel();
                }
                else
                {
                    Handheld.Vibrate();
                    m_CurrentTry++;
                }
            }
            m_CurrentUserJoint = joint;
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

        public void StartGame()
        {
            if (m_IsGameStarted)
                return;

            UserRoboticArm.StopMotion();
            UserRoboticArm.gameObject.SetActive(false);
            TryingInstruction.SetActive(false);
            GameInstruction.SetActive(true);
            CountdownManager.Instance.StartCountdown(3f, StartGameInternal);
        }

        private void StartGameInternal()
        {
            m_CurrentTime = TimePerGame;
            m_CurrentTry = 1;
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = 0;
            m_Levels.Shuffle();
            m_Levels[m_CurrentLevelIndex].Show();          
            m_IsGameStarted = true;
        }

        private void StopGame()
        {
            if (!m_IsGameStarted)
                return;

            m_IsGameStarted = false;
            TryingInstruction.SetActive(true);
            GameInstruction.SetActive(false);
            UserRoboticArm.gameObject.SetActive(true);           
            m_Levels[m_CurrentLevelIndex].Hide();
            PointsText.text = TimePerGame.ToString("n0");
        }

        private void ShowLevel(int index)
        {
            m_CurrentTry = 1;
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = MathUtility.WrapArrayIndex(index, m_Levels.Count); ;
            m_Levels[m_CurrentLevelIndex].Show();
        }

        private void ResetLevels()
        {
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = 0;
            m_Levels.Shuffle();
            m_Levels[m_CurrentLevelIndex].Show();
        }

        private void ShowNextLevel()
        {
            if (m_CurrentLevelIndex == m_Levels.Count - 1)
            {
                ResetLevels();
                return;
            }
            
            ShowLevel(m_CurrentLevelIndex + 1);
        }
    }
}
