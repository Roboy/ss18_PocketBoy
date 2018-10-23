﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using UnityEngine.UI;
using System;


namespace Pocketboy.PitchPlatformer
{
    /// <summary>
    /// CalibrationManager listens to user via microphone input to calibrate the lower and higher boundary of the user pitch.
    /// </summary>
    [RequireComponent(typeof(MicrophoneFeed))]
    public class CalibrationManager : Singleton<CalibrationManager>
    {
        [SerializeField]
        private GameObject CalibrationUI;

        [SerializeField]
        private Button CalibrateHighPitchButton;

        [SerializeField]
        private Button CalibrateLowPitchButton;

        [SerializeField]
        private Slider HighPitchVisualization;

        [SerializeField]
        private Slider LowPitchVisualization;

        [SerializeField]
        private Slider LowPitchDuration;

        [SerializeField]
        private Slider HighPitchDuration;

        [SerializeField]
        private Slider PitchVisualization;

        [SerializeField]
        private Slider HighPitchArea;

        [SerializeField]
        private Slider LowPitchArea;

        [SerializeField]
        private float m_CalibrationDuration;

        [SerializeField]
        private GameObject CalibrationWindow;

        [SerializeField]
        private GameObject CalibrationTestWindow;

        [SerializeField]
        private Button ShowCalibrationButton;

        [SerializeField]
        private Button ShowCalibrationTestButton;

        [SerializeField]
        private Button FinishCalibrationButton;

        public int CalibratedHighNote { get; private set; }

        public int CalibratedLowNote { get; private set; }

        private float m_HighPitchReference = 523.25f; // Hertz value of C5

        private float m_LowPitchReference = 82.407f; // Hertz value of E2

        /// <summary>
        /// Calibrated high pitch as reference to shift user pitch from [m_CalibratedLowPitch, m_CalibratedHighPitch]
        /// </summary>
        private float m_CalibratedHighPitch;

        /// <summary>
        /// Calibrated low pitch as reference to shift user pitch from [m_CalibratedLowPitch, m_CalibratedHighPitch]
        /// </summary>
        private float m_CalibratedLowPitch;

        /// <summary>
        /// Allows to collect the samples from the microphone input
        /// </summary>
        private MicrophoneFeed m_MicrophoneFeed;

        /// <summary>
        /// Pitch tracker proccess the samples from the microphone feed
        /// </summary>
        private PitchTracker m_PitchTracker;

        /// <summary>
        /// Is used to check after calibration of low/high pitch if the opposite pitch is calibrated already to allow the user to proceed to test the calibration
        /// </summary>
        private bool m_OnePitchCalibrated;

        /// <summary>
        /// In calibration test mode this is set to true to collect samples from the microphone feed and process it in the pitch tracker
        /// </summary>
        private bool m_IsListeningForPitches;

        void Start()
        {
            m_MicrophoneFeed = GetComponent<MicrophoneFeed>();
            m_MicrophoneFeed.StopRecording(); // the user should press a button to calibrate, so we do not need to record all the time

            m_PitchTracker = new PitchTracker();
            m_PitchTracker.SampleRate = AudioSettings.outputSampleRate;

            m_CalibratedLowPitch = m_LowPitchReference;
            m_CalibratedHighPitch = m_HighPitchReference;

            CalibrateHighPitchButton.onClick.AddListener(CalibrateHighPitch);
            CalibrateLowPitchButton.onClick.AddListener(CalibrateLowPitch);

            ShowCalibrationTestButton.gameObject.SetActive(false); // at the start the user has not calibrated his pitch yet, so he should not be able to test it

            ShowCalibrationTestButton.onClick.AddListener(ShowCalibrationTestWindow);
            ShowCalibrationButton.onClick.AddListener(ShowCalibrationWindow);

            FinishCalibrationButton.onClick.AddListener(SaveCalibration);

            LoadCalibration();            
        }

        /// <summary>
        /// Loads a saved calibration if possible from PlayerPrefs.
        /// </summary>
        private void LoadCalibration()
        {
            if (PlayerPrefs.GetFloat("LowPitchValue") == default(float)) // if not saved yet, GetFloat returns default value
                return;

            m_CalibratedLowPitch = PlayerPrefs.GetFloat("LowPitchValue");
            m_CalibratedHighPitch = PlayerPrefs.GetFloat("HighPitchValue");

            LowPitchVisualization.value = MathUtility.ConvertRange(m_LowPitchReference, m_HighPitchReference, 0f, 1f, m_CalibratedLowPitch);
            HighPitchVisualization.value = MathUtility.ConvertRange(m_LowPitchReference, m_HighPitchReference, 0f, 1f, m_CalibratedHighPitch);
        }

        /// <summary>
        /// Saves the current calibration in PlayerPrefs.
        /// </summary>
        private void SaveCalibration()
        {
            CalibrationUI.SetActive(false);
            ForceScreenOrientation.Instance.ResetScreenOrientation(); // the calibration should run in portrait mode, reset this to previous screen orientation when finished

            PlayerPrefs.SetFloat("LowPitchValue", m_CalibratedLowPitch);
            PlayerPrefs.SetFloat("HighPitchValue", m_CalibratedHighPitch);

            ShowCalibrationTestButton.gameObject.SetActive(true);
            m_OnePitchCalibrated = true;
        }

        private void CalibrateHighPitch()
        {
            m_MicrophoneFeed.StartRecording();
            StartCoroutine(GetPitchAverage((value) => 
            {
                if (float.IsNaN(value) ||  value < m_CalibratedLowPitch)
                {
                    WarningManager.Instance.ShowWarning("Please try to hum in a higher voice.");
                    ResetCalibration(HighPitchDuration);
                    return;
                }
                m_CalibratedHighPitch = value;
                int midiNote, cents;
                bool isMidiNote = PitchDsp.PitchToMidiNote(m_CalibratedHighPitch, out midiNote, out cents);
                if (isMidiNote)
                {
                    CalibratedHighNote = midiNote;
                }

                ResetCalibration(HighPitchDuration, HighPitchVisualization);
                CheckForCalibrationTest();
            }, m_LowPitchReference, m_HighPitchReference, HighPitchDuration));
        }

        private void CalibrateLowPitch()
        {
            m_MicrophoneFeed.StartRecording();
            StartCoroutine(GetPitchAverage((value) => 
            {
                if (float.IsNaN(value) || value > m_CalibratedHighPitch)
                {
                    WarningManager.Instance.ShowWarning("Please try to hum in a deeper voice.");
                    ResetCalibration(LowPitchDuration);
                    return;
                }
                m_CalibratedLowPitch = value;
                int midiNote, cents;
                bool isMidiNote = PitchDsp.PitchToMidiNote(m_CalibratedLowPitch, out midiNote, out cents);
                if (isMidiNote)
                {
                    CalibratedLowNote = midiNote;
                }

                ResetCalibration(LowPitchDuration, LowPitchVisualization);
                CheckForCalibrationTest();
            }, m_LowPitchReference, m_HighPitchReference, LowPitchDuration));
        }

        /// <summary>
        /// Is used after calibration of low/high pitch to reset the visual parts while calibrating.
        /// </summary>
        /// <param name="durationSlider"></param>
        /// <param name="pitchSlider"></param>
        private void ResetCalibration(Slider durationSlider, Slider pitchSlider = null)
        {
            m_MicrophoneFeed.StopRecording();
            if (pitchSlider != null)
            {
                pitchSlider.gameObject.SetActive(true);
                pitchSlider.value = PitchVisualization.value;
            }            
            PitchVisualization.value = 0.5f;
            durationSlider.value = 0f;
        }

        /// <summary>
        /// If one pitch (high or low) is not calibrated yet, just save that this pitch is calibrated, otherwise calibration is complete and can be tested
        /// </summary>
        private void CheckForCalibrationTest()
        {
            // if one pitch (high or low) is not calibrated yet, just save that this pitch is calibrated, otherwise calibration is complete and can be tested
            if (!m_OnePitchCalibrated)
                m_OnePitchCalibrated = true;
            else
                ShowCalibrationTestButton.gameObject.SetActive(true);

            m_IsListeningForPitches = false;
        }

        /// <summary>
        /// In the calibration test window the user moves the pitch bar with his pitch into areas corresponding to the calibration. If the user can move the bar comfortably he can proceed
        /// to finish the calibration.
        /// </summary>
        private void ShowCalibrationTestWindow()
        {
            CalibrationWindow.SetActive(false);
            CalibrationTestWindow.SetActive(true);
            ShowCalibrationButton.gameObject.SetActive(true);
            ShowCalibrationTestButton.gameObject.SetActive(false);
            m_MicrophoneFeed.StartRecording();

            // LowPitchArea and visualization start from the same point but area can only be max half the size so
            // we have to multiply the area by factor 2 and add a small offset so the threshold for a successful calibration is smaller than the lowest pitch
            LowPitchArea.value = (LowPitchVisualization.value * 2f) + 0.1f;

            // HighPitchArea and visualization start from the opposite point but area can only be max half the size so
            // we have to multiply the area by factor 2 and add a small offset so the threshold for a successful calibration is smaller than the higher pitch
            HighPitchArea.value =(1f - HighPitchVisualization.value) * 2f + 0.1f;

            PitchTracker.PitchDetectedHandler PitchDetected = (sender, pitchRecord) =>
            {
                if (pitchRecord.Pitch < 20.0f) // pitches under 20Hz should not occur normally, as it is already not a MIDI note
                    return;

                PitchVisualization.value = MathUtility.ConvertRange(m_CalibratedLowPitch, m_CalibratedHighPitch, 0f, 1f, pitchRecord.Pitch);
            };

            m_PitchTracker.PitchDetected += PitchDetected;
            StartCoroutine(ListenForPitches());
        }

        /// <summary>
        /// Listens to microphone input and feeds its samples into the pitch tracker.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ListenForPitches()
        {
            m_IsListeningForPitches = true;
            while (m_IsListeningForPitches)
            {
                m_PitchTracker.ProcessBuffer(m_MicrophoneFeed.GetSamples());
                yield return null;
            }
        }

        /// <summary>
        /// Returns back from calibration test mode to the initial window.
        /// </summary>
        private void ShowCalibrationWindow()
        {
            CalibrationWindow.SetActive(true);
            CalibrationTestWindow.SetActive(false);
            ShowCalibrationButton.gameObject.SetActive(false);
            ShowCalibrationTestButton.gameObject.SetActive(true);
            m_MicrophoneFeed.StopRecording();
        }

        /// <summary>
        /// Calculates the pitch average proccessed by PitchTracker and adjusts the pitch bar.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="durationSlider"></param>
        /// <returns></returns>
        IEnumerator GetPitchAverage(Action<float> onComplete, float min, float max, Slider durationSlider)
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
                durationSlider.value = Mathf.Lerp(0f, 1f, currentDuration / m_CalibrationDuration);
                m_PitchTracker.ProcessBuffer(m_MicrophoneFeed.GetSamples());
                if (m_PitchTracker.CurrentPitchRecord.Pitch > 19f)
                    PitchVisualization.value = MathUtility.ConvertRange(min, max, 0f, 1f, pitches / pitchCount);
                currentDuration += Time.deltaTime;
                yield return null;
            }

            m_PitchTracker.PitchDetected -= PitchDetected;           
            onComplete(pitches / pitchCount);
        }
    }

}

