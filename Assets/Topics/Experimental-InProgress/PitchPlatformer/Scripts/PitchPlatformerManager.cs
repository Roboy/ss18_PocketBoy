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
        private float HeightRange = 0.5f;

        [SerializeField]
        private PlatformPlayer Player;

        [SerializeField]
        private Transform SpawnPoint;

        [SerializeField]
        private List<PitchPlatformLevel> Levels = new List<PitchPlatformLevel>();

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

        void Start()
        {
#if !UNITY_EDITOR
            LevelsParent.transform.parent = RoboyManager.Instance.ARAnchor.transform;
            LevelsParent.transform.position = RoboyManager.Instance.transform.position + RoboyManager.Instance.transform.right * -0.5f + RoboyManager.Instance.transform.up * (HeightRange * 0.1f);
#endif
            PitchRecognizer = new PitchTracker();
            PitchRecognizer.SampleRate = AudioSettings.outputSampleRate;

            PitchRecognizer.PitchDetected += (sender, pitch) => { PitchValueText.text = pitch.MidiNote.ToString(); };

            PitchPlatformerEvents.ReachedGoalEvent += () => LevelUI.SetActive(true);
            SetupLevels();
            StartGame();
        }

        public void StartGame()
        {
            if (Levels.Count < 1)
                return;
           
            Levels[0].Show();
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
            PitchVisualization.value = pitchValue;
        }
        
        private void SetupLevels()
        {
            Player.SetSpawnPosition(SpawnPoint.position);
            foreach (var level in Levels)
            {
                level.Setup(HeightRange, AccuracyThreshold, PlatformLengthPerSecond, Player);
                level.Hide();
            }
        }

        private void GoToLevel(int levelIndex)
        {
            Levels[m_CurrentLevelIndex].Hide();
            m_CurrentLevelIndex = MathUtility.WrapArrayIndex(levelIndex, Levels.Count);
            Levels[m_CurrentLevelIndex].Show();            
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
    }
}
