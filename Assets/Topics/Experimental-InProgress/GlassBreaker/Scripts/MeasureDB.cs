﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MeasureDB : MonoBehaviour {

    //Microphone Input
    AudioSource audioSource;
    public AudioClip audioClip;
    public bool useMicrophone;
    public string selectedDevice;
    public AudioMixerGroup mixerGroupMicrophone, mixerGroupMaster;
    private float m_dbTreshold = -160.0f;

    public float RmsValue;
    public float DbValue;
    public float PitchValue;

    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    private Vector3 m_originalScale;
    private int m_LastCrackTier;
    private int m_CurrentCrackTier = 0;
    private bool m_CheckingForCracks = false;
    private bool m_VolumeCalibrated = false;

    void Start()
    {
        m_LastCrackTier = m_CurrentCrackTier;
        m_originalScale = transform.localScale;
        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;
        audioSource = GetComponent<AudioSource>();
        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                
                selectedDevice = Microphone.devices[0].ToString();
                audioSource.outputAudioMixerGroup = mixerGroupMicrophone;
                audioSource.clip = Microphone.Start(selectedDevice, true, 600, AudioSettings.outputSampleRate);
            }
            else
            {
                useMicrophone = false;
            }
        }
        if (!useMicrophone)
        {
            audioSource.outputAudioMixerGroup = mixerGroupMaster;
            audioSource.clip = audioClip;
        }

        audioSource.Play();
    }

    void Update()
    {
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

        if (DbValue >= m_dbTreshold && DbValue < m_dbTreshold + 5.0f)
            volumeTier = 0;
        if (DbValue >= m_dbTreshold + 5.0f && DbValue < m_dbTreshold + 15.0f)
            volumeTier = 1;
        if (DbValue >= m_dbTreshold + 15.0f && DbValue < m_dbTreshold + 27.5f)
            volumeTier = 2;
        if (DbValue >= m_dbTreshold + 27.5f && DbValue < m_dbTreshold + 32.5f)
            volumeTier = 3;
        if (DbValue >= m_dbTreshold + 32.5f && DbValue < m_dbTreshold + 35.0f)
            volumeTier = 4;
        if (DbValue >= m_dbTreshold + 35.0f && DbValue < m_dbTreshold + 36.5f)
            volumeTier = 5;
        if (DbValue >= m_dbTreshold + 36.5f && DbValue < m_dbTreshold + 38.0)
            volumeTier = 6;
        if (DbValue >= m_dbTreshold + 38.0f)
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
        GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
                                            // get sound spectrum
        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;

            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1)
        { // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency

        if (DbValue > m_dbTreshold)
        { transform.localScale = m_originalScale + (1.0f - DbValue / m_dbTreshold) * m_originalScale; }
        else
        {
          transform.localScale = m_originalScale;
        }
    }

    public void CalibrateMicrophone()
    {
        m_VolumeCalibrated = true;
        m_CurrentCrackTier = 0;
        m_dbTreshold = DbValue + 5.0f;
        ScreenHealthController.Instance.SetScreenCrackTier(0);
    }

    public void ResetTier()
    {
        m_CurrentCrackTier = 0;
        ScreenHealthController.Instance.SetScreenCrackTier(0);
    }
}

