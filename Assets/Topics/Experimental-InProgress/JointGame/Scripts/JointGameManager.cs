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
        private float WarningTime;

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

        [SerializeField]
        private TextMeshProUGUI HighscoreText;

        [Header("Other stuff")]

        [SerializeField]
        private RoboticArmController RoboticArm;

        [SerializeField]
        private Transform LevelsParent;

        [SerializeField]
        private Button PlayButton;

        [SerializeField]
        private Button StopButton;

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

        private int m_CurrentLevelPointHits;

        private bool m_CountdownWarningTriggered;

        private float m_Highscore;

        private static string m_HighscorePlayerPrefsName = "JointGameHighscore";

        private void Awake()
        {
            AddSubscribers();
            SetupLevels();
            LoadHighscore();

            if (LevelManager.InstanceExists)
            {
                LevelManager.Instance.RegisterGameObjectWithRoboy(LevelsParent.gameObject, Vector3.right * 0.2f + Vector3.forward * 0.3f, Quaternion.identity);
                LevelsParent.forward = -LevelsParent.forward;
            }

            TimeText.text = TimePerGame.ToString("n0");
        }

        private void OnDestroy()
        {
            SaveHighscore();
        }

        private void Update()
        {
            if (!m_IsGameStarted)
                return;

            m_CurrentTime -= Time.deltaTime;
            TimeText.text = m_CurrentTime.ToString("n1");

            if (m_CurrentTime < WarningTime && !m_CountdownWarningTriggered)
            {
                CountdownManager.Instance.StartWarningCountdown(WarningTime, StopGame);
                m_CountdownWarningTriggered = true;
            }

            PointsText.text = m_CurrentPoints.ToString("n0");
        }

        public void PointWasHit()
        {
            AudioSourcesManager.Instance.PlaySound("CoinPickup");
            IncreaseUserPoints();
            m_CurrentLevelPointHits++;
            if (m_CurrentLevelPointHits == m_Levels[m_CurrentLevelIndex].Points)
            {
                ShowNextLevel();
            }
        }

        private void AddSubscribers()
        {
            LinearJointButton.onClick.AddListener(() => 
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                if (m_IsGameStarted)
                    m_CurrentTry++;

                RoboticArm.LinearMotion();
            });
            OrthogonalJointButton.onClick.AddListener(() =>
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                if (m_IsGameStarted)
                    m_CurrentTry++;

                RoboticArm.OrthogonalMotion();
            });
            RotationalJointButton.onClick.AddListener(() =>
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                if (m_IsGameStarted)
                    m_CurrentTry++;

                RoboticArm.RotationalMotion();
            });
            TwistJointButton.onClick.AddListener(() =>
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                if (m_IsGameStarted)
                    m_CurrentTry++;

                RoboticArm.TwistMoting();
            });
            RevolvingJointButton.onClick.AddListener(() =>
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                if (m_IsGameStarted)
                    m_CurrentTry++;

                RoboticArm.RevolvingMotion();
            });

            PlayButton.onClick.AddListener(() => 
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                PlayButton.gameObject.SetActive(false);
                StopButton.gameObject.SetActive(true);
                StartGame();
            });

            StopButton.onClick.AddListener(() => 
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                PlayButton.gameObject.SetActive(true);
                StopButton.gameObject.SetActive(false);
                StopGame();
            });
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
            if (m_IsGameStarted)
                return;

            RoboticArm.StopMotion();
            CountdownManager.Instance.StartCountdownBlockingInput(3f, StartGameInternal);
        }



        private void StartGameInternal()
        {
            m_CountdownWarningTriggered = false;
            m_CurrentTime = TimePerGame;
            m_CurrentTry = 0;
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
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentPoints = 0f;
            PointsText.text = "0";
            TimeText.text = TimePerGame.ToString("n0");
            StopButton.gameObject.SetActive(false);
            PlayButton.gameObject.SetActive(true);
        }

        private void ShowLevel(int index)
        {
            m_CurrentTry = 0;
            m_CurrentLevelPointHits = 0;
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = MathUtility.WrapArrayIndex(index, m_Levels.Count); ;
            m_Levels[m_CurrentLevelIndex].Show();
        }

        private void ResetLevels()
        {
            m_CurrentLevelPointHits = 0;
            m_CurrentTry = 0;
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

        private void IncreaseUserPoints()
        {
            m_CurrentPoints += MaxPointsPerRound / Mathf.Max(1f, m_CurrentTry);
            PointsText.text = m_CurrentPoints.ToString();
            UpdateHighscore();
        }

        private void UpdateHighscore()
        {
            if (m_CurrentPoints <= m_Highscore)
                return;

            m_Highscore = m_CurrentPoints;
            HighscoreText.text = m_Highscore.ToString("n0");
        }

        private void LoadHighscore()
        {
            if (PlayerPrefs.GetFloat(m_HighscorePlayerPrefsName) == default(float)) // if not saved yet, GetInt returns default value
                return;

            m_Highscore = PlayerPrefs.GetFloat(m_HighscorePlayerPrefsName);
            HighscoreText.text = m_Highscore.ToString("n0");
        }

        private void SaveHighscore()
        {
            if (PlayerPrefs.GetFloat(m_HighscorePlayerPrefsName) >= m_Highscore)
                return;

            PlayerPrefs.SetFloat(m_HighscorePlayerPrefsName, m_Highscore);
        }
    }
}
