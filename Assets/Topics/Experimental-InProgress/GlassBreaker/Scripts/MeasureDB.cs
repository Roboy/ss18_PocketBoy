using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Pocketboy.Common;

namespace Pocketboy.Glassbreaker {

    public class MeasureDB : MonoBehaviour {

        
        public float m_dbTreshold = -160.0f;

        public float RmsValue;
        public float DbValue;
        public float PitchValue;

        private const int QSamples = 1024;
        private const float RefValue = 0.1f;
        private const float Threshold = 0.02f;

        float[] _samples;
        private float[] _spectrum;
        private float _fSample;
        private float _weightFactor = 0.0f;

        private Vector3 m_originalScale;
        private int m_LastCrackTier;
        private int m_CurrentCrackTier = 0;
        private bool m_CheckingForCracks = false;
        private bool m_VolumeCalibrated = false;

        private bool m_Initialized = false;

        private float DbOffset = 80.0f;

        void Start()
        {

        }

        void Update()
        {
            if (!m_Initialized)
            {
                Initialize();
            }

            AnalyzeSound();

            if (!m_CheckingForCracks && m_VolumeCalibrated)
            {
                StartCoroutine(CheckForCracks());
            }

        }

        private IEnumerator CheckForCracks()
        {

            m_CheckingForCracks = true;
            int volumeTier = 0;
            _weightFactor = 1.0f - (m_dbTreshold / 80.0f);

            if (DbValue >= m_dbTreshold && DbValue < m_dbTreshold + 40.0f * _weightFactor)
                volumeTier = 0;
            if (DbValue >= m_dbTreshold + 40.0f * _weightFactor && DbValue < m_dbTreshold + 50.0f * _weightFactor)
                volumeTier = 1;
            if (DbValue >= m_dbTreshold + 50.0f * _weightFactor && DbValue < m_dbTreshold + 60.0f * _weightFactor)
                volumeTier = 2;
            if (DbValue >= m_dbTreshold + 60.0f * _weightFactor && DbValue < m_dbTreshold + 70.0f * _weightFactor)
                volumeTier = 3;
            if (DbValue >= m_dbTreshold + 70.0f * _weightFactor && DbValue < m_dbTreshold + 80.0f * _weightFactor)
                volumeTier = 4;
            if (DbValue >= m_dbTreshold + 80.0f * _weightFactor && DbValue < m_dbTreshold + 85.0f * _weightFactor)
                volumeTier = 5;
            if (DbValue >= m_dbTreshold + 85.0f * _weightFactor && DbValue < m_dbTreshold + 90.0 * _weightFactor)
                volumeTier = 6;
            if (DbValue >= m_dbTreshold + 90.0f * _weightFactor)
                volumeTier = 7;




            if (volumeTier > m_CurrentCrackTier)
            {
                m_CurrentCrackTier += 1;
                ScreenHealthController.Instance.SetScreenCrackTier(m_CurrentCrackTier);

            }
            m_CheckingForCracks = false;
            yield return null;

        }

        void AnalyzeSound()
        {
            //GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
            _samples = MicrophoneManager.Instance.GetSamples();

            if (_samples == null)
            {
                return;
            }

            int i;
            float sum = 0;
            for (i = 0; i < QSamples; i++)
            {
                sum += _samples[i] * _samples[i]; // sum squared samples
            }
            RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
            DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate db
            DbValue += DbOffset;
            if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
                                                // get sound spectrum
            ScreenHealthController.Instance.PrintDebugText(DbValue.ToString());
            
            //GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
            //float maxV = 0;
            //var maxN = 0;
            //for (i = 0; i < QSamples; i++)
            //{ // find max 
            //    if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
            //        continue;

            //    maxV = _spectrum[i];
            //    maxN = i; // maxN is the index of max
            //}
            //float freqN = maxN; // pass the index to a float variable
            //if (maxN > 0 && maxN < QSamples - 1)
            //{ // interpolate index using neighbours
            //    var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            //    var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            //    freqN += 0.5f * (dR * dR - dL * dL);
            //}
            //PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency


        }

        public IEnumerator CalibrateMicrophone(float calibTime)
        {
            ScreenHealthController.Instance.SetScreenCrackTier(0);
            m_CurrentCrackTier = 0;
            int counter = 0;
            float timer = 0.0f;
            float db_values = 0.0f;

            while (timer < calibTime)
            {
                ScreenHealthController.Instance.FillSlider(timer / calibTime);
                ScreenHealthController.Instance.FadeRoboySprite(1.0f - (timer / calibTime));
                db_values += DbValue;
                counter++;
                timer += Time.deltaTime;
                yield return null;
            }

            m_dbTreshold = db_values / (float) counter;
            m_VolumeCalibrated = true;
            ScreenHealthController.Instance.ToggleButtons("ON");
            ScreenHealthController.Instance.ToggleCalibrationInstructions("OFF");
            ScreenHealthController.Instance.ResetCalibrationInstructions();
            yield return null;
        }

        public void ResetTier()
        {
            m_CurrentCrackTier = 0;
            ScreenHealthController.Instance.SetScreenCrackTier(0);
            
        }

        private void Initialize()
        {
            m_LastCrackTier = m_CurrentCrackTier;
            m_Initialized = true;
        }

        public void ResetCalibrationStatus()
        {
            m_VolumeCalibrated = false;
        }
    }

}