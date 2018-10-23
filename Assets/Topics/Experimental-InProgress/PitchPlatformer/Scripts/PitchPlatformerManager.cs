using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using UnityEngine.UI;

namespace Pocketboy.PitchPlatformer
{
    [RequireComponent(typeof(MicrophoneFeed))]
    public class PitchPlatformerManager : Singleton<PitchPlatformerManager>
    {
        [SerializeField]
        private Text PitchValueText;

        [SerializeField]
        private float HeightRange;

        [SerializeField]
        private List<PitchPlatform> Platforms = new List<PitchPlatform>();

        /// <summary>
        /// This value translates the length of a platform to the needed duration of recognized pitches in seconds, meaning with a value of 0.1f a platform with length 0.5f needs 5 seconds
        /// of correct pitch recognition in order to finish.
        /// </summary>
        [SerializeField]
        private float PlatformLengthPerSecond = 0.1f;

        public PitchTracker PitchRecognizer { get; private set; }

        /// <summary>
        /// This value controls the needed accuracy (as percentage) of a platform, e.g. a platform with note C4 accepts notes from A3 to E4.
        /// </summary>
        [SerializeField]
        private int AccuracyThreshold = 2;

        private int m_CurrentPlatformIndex = 0;

        /// <summary>
        /// Allows to collect the samples from the microphone input
        /// </summary>
        private MicrophoneFeed m_MicrophoneFeed;

        private bool m_IsRunning;
       

        void Start()
        {
            PitchRecognizer = new PitchTracker();
            PitchRecognizer.SampleRate = AudioSettings.outputSampleRate;

            m_MicrophoneFeed = GetComponent<MicrophoneFeed>();
            m_MicrophoneFeed.StopRecording(); // the user should press a button to calibrate, so we do not need to record all the time

            PitchRecognizer.PitchDetected += (sender, pitch) => { PitchValueText.text = pitch.MidiNote.ToString(); };
        }

        private void FixedUpdate()
        {
            if (!m_IsRunning)
                return;

            PitchRecognizer.ProcessBuffer(m_MicrophoneFeed.GetSamples());
        }

        public void StartGame()
        {
            if (Platforms.Count < 1)
                return;

            SetupPlatforms();
            m_MicrophoneFeed.StartRecording();
            Platforms[0].StartListen();
            m_IsRunning = true;
            
        }

        void SetupPlatforms()
        {
            foreach (var platform in Platforms)
            {
                // get height value between 0 and 1
                float normalizedHeightValue = MathUtility.NormalizeValue(-HeightRange, HeightRange, platform.transform.localPosition.y);
                // get the corresponding pitch value depending on height value
                //float platformPitch = Mathf.Lerp(CalibrationManager.Instance.LowPitch, CalibrationManager.Instance.HighPitch, normalizedHeightValue);

                // DEBUG
                int platformNote = (int)Mathf.Lerp(36, 69, normalizedHeightValue);
                platform.Setup(platformNote, AccuracyThreshold, PlatformLengthPerSecond);
            }
        }
    }
}
