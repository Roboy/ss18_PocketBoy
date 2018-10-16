using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using UnityEngine.UI;
using System;


namespace Pocketboy.PitchPlatformer
{
    [RequireComponent(typeof(MicrophoneFeed))]
    public class CalibrationManager : Singleton<CalibrationManager>
    {
        [SerializeField]
        private Button CalibrateHighPitchButton;

        [SerializeField]
        private Button CalibrateLowPitchButton;

        [SerializeField]
        private Slider PitchVisualization;

        [SerializeField]
        private float m_CalibrationDuration;

        private float m_CalibratedHighPitch = 523.25f; // Hertz value of C5

        private float m_CalibratedLowPitch = 82.407f; // Hertz value of E2

        private bool m_Calibrated;

        private MicrophoneFeed m_MicrophoneFeed;

        private PitchTracker m_PitchTracker;

        private bool m_IsCalibrated;

        // Use this for initialization
        void Start()
        {
            m_MicrophoneFeed = GetComponent<MicrophoneFeed>();
            m_MicrophoneFeed.StopRecording(); // the user should press a button to calibrate, so we do not need to record all the time

            m_PitchTracker = new PitchTracker();
            m_PitchTracker.SampleRate = AudioSettings.outputSampleRate;

            CalibrateHighPitchButton.onClick.AddListener(CalibrateHighPitch);
            CalibrateLowPitchButton.onClick.AddListener(CalibrateLowPitch);
        }

        private void CalibrateHighPitch()
        {
            m_MicrophoneFeed.StartRecording();
            StartCoroutine(GetNoteAverage((value) => { m_CalibratedHighPitch = value; m_MicrophoneFeed.StopRecording(); Debug.Log(value); }));
        }

        private void CalibrateLowPitch()
        {
            m_MicrophoneFeed.StartRecording();
            StartCoroutine(GetNoteAverage((value) => { m_CalibratedLowPitch = value; m_MicrophoneFeed.StopRecording(); Debug.Log(value); }));
        }

        IEnumerator GetNoteAverage(Action<float> onComplete)
        {
            float pitches = 0f;
            int pitchCount = 0;
            PitchTracker.PitchDetectedHandler PitchDetected = (sender, pitchRecord) =>
            {
                if (pitchRecord.Pitch < 20.0f) // pitches under 20Hz should not occur normally, as it is already not a MIDI note
                    return;

                pitches += pitchRecord.Pitch;
                pitchCount++;
               
            };

            m_PitchTracker.PitchDetected += PitchDetected;

            float currentDuration = 0f;
            while (currentDuration < m_CalibrationDuration)
            {
                m_PitchTracker.ProcessBuffer(m_MicrophoneFeed.GetSamples());
                if(m_PitchTracker.CurrentPitchRecord.Pitch > 19f)
                    PitchVisualization.value = PitchToCalibratedRange(m_PitchTracker.CurrentPitchRecord.Pitch);
                currentDuration += Time.deltaTime;
                yield return null;
            }

            m_PitchTracker.PitchDetected -= PitchDetected;
            
            onComplete(pitches / pitchCount);
        }

        private float PitchToCalibratedRange(float pitch)
        {
            return Mathf.Max(0f, pitch - m_CalibratedLowPitch) / (m_CalibratedHighPitch - m_CalibratedLowPitch);
        }






    }

}

