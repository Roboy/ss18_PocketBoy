using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using UnityEngine.UI;

namespace Pocketboy.PitchPlatformer
{
    public class PitchPlatformerManager : Singleton<PitchPlatformerManager>
    {
        [SerializeField]
        private Text PitchValueText;

        [SerializeField]
        private float HeightRange = 0.3f;

        [SerializeField]
        private PlatformPlayer Player;

        [SerializeField]
        private Transform SpawnPoint;

        [SerializeField]
        private Slider PitchVisualization;

        /// <summary>
        /// This value translates the length of a platform to the needed duration of recognized pitches in seconds, meaning with a value of 0.1f a platform with length 0.5f needs 5 seconds
        /// of correct pitch recognition in order to finish.
        /// </summary>
        [SerializeField]
        private float PlatformLengthPerSecond = 0.1f;

        /// <summary>
        /// This value controls the needed accuracy (as percentage) of a platform, e.g. a platform with note C4 accepts notes from A3 to E4.
        /// </summary>
        [SerializeField]
        private int AccuracyThreshold = 2;

        [SerializeField]
        private GameObject LevelUI;

        [SerializeField]
        private GameObject LevelsParent;

        public PitchTracker PitchRecognizer { get; private set; }

        private int m_CurrentLevelIndex = 0;

        private PitchPlatformLevel[] m_Levels;

        private IEnumerator m_AnimationCoroutine;

        void Start()
        {
            if (LevelManager.InstanceExists)
            {
                LevelManager.Instance.RegisterGameObjectWithRoboy(LevelsParent, new Vector3(-1f, 0.2f, HeightRange * 1.1f));
                LevelsParent.transform.right = RoboyManager.Instance.transform.right;
            }

            PitchRecognizer = new PitchTracker();
            PitchRecognizer.SampleRate = AudioSettings.outputSampleRate;

            PitchRecognizer.PitchDetected += (sender, pitch) => { PitchValueText.text = pitch.MidiNote.ToString(); };

            PitchPlatformerEvents.ReachedGoalEvent += () => LevelUI.SetActive(true);
            m_Levels = LevelsParent.GetComponentsInChildren<PitchPlatformLevel>(true);
        }

        public void PauseGame()
        {
            m_Levels[m_CurrentLevelIndex].Hide();
        }

        public void ResumeGame()
        {
            SetupLevels();
            GoToLevel(m_CurrentLevelIndex);
        }       

        public void RepeatLevel()
        {
            LevelUI.SetActive(false);
            GoToLevel(m_CurrentLevelIndex);
        }

        public void NextLevel()
        {
            LevelUI.SetActive(false);
            GoToLevel(m_CurrentLevelIndex + 1);
        }

        public void SetPitchValue(int requiredNote, int actualNote)
        {
            float pitchValue = -1f;
            if (requiredNote == actualNote) // actual note is hit
            {             
                pitchValue = 0.5f;
            }
            else if (actualNote < requiredNote) // lower than the required note
            {
                if (actualNote >= requiredNote - AccuracyThreshold) // within accepted bounds
                {
                    pitchValue = 0.3f;
                }
                else // lower than the bottom threshold
                {
                    pitchValue = 0.1f;
                }
            }
            else
            {
                if (actualNote <= requiredNote + AccuracyThreshold) // within accepted bounds
                {
                    pitchValue = 0.7f;
                }
                else // higher than the accepted threshold
                {
                    pitchValue = 0.9f;
                }
            }
            if (m_AnimationCoroutine != null)
                StopCoroutine(m_AnimationCoroutine);

            StartCoroutine(PitchVisualizationAnimation(pitchValue));
        }

        private void SetupLevels()
        {
            //Player.SetSpawnPosition(SpawnPoint.position);
            foreach (var level in m_Levels)
            {
                level.Setup(HeightRange, AccuracyThreshold, PlatformLengthPerSecond, Player);
                level.Hide();
            }
        }

        private void GoToLevel(int levelIndex)
        {
            m_Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = MathUtility.WrapArrayIndex(levelIndex, m_Levels.Length);
            m_Levels[m_CurrentLevelIndex].Show();
        }

        private IEnumerator PitchVisualizationAnimation(float value)
        {
            if (PitchVisualization.value == value)
                yield break;

            float duration = 0.3f;
            float currentDuration = 0f;
            float initValue = PitchVisualization.value;
            while (currentDuration < duration)
            {
                currentDuration += Time.deltaTime;
                PitchVisualization.value = Mathf.Lerp(initValue, value, currentDuration / duration);
                yield return null;
            }
            PitchVisualization.value = value;
        }
    }
}