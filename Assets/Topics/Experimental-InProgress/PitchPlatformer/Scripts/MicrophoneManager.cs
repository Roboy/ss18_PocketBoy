using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Pocketboy.Common;

namespace Pocketboy.Common
{
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneManager : Singleton<MicrophoneManager>
    {
        [SerializeField]
        private bool StartOnAwake;

        private AudioSource m_AudioSource;

        private string m_Device;

        private float[] m_Samples = new float[1024];

        // Use this for initialization
        void Awake()
        {
            foreach (var device in Microphone.devices)
            {
                m_Device = device;
                break;
            }

            if (string.IsNullOrEmpty(m_Device))
            {
                Debug.Log("No Microphone found!");
                return;
            }
            
            m_AudioSource = GetComponent<AudioSource>();
            
            if (StartOnAwake)
                StartRecording();
        }

        public void OnDestroy()
        {
            StopRecording();
        }

        public void StartRecording()
        {
            if (string.IsNullOrEmpty(m_Device))
            {
                Debug.Log("No Microphone found!");
                return;
            }
            if (Microphone.IsRecording(m_Device))
                StopRecording();

            if (m_AudioSource.isPlaying)
                m_AudioSource.Stop();

            m_AudioSource.clip = null;
            m_AudioSource.loop = true;

            m_AudioSource.clip = Microphone.Start(m_Device, true, 1, AudioSettings.outputSampleRate);
            m_AudioSource.Play();
            
            int dspBufferSize, dspNumBuffers;
            AudioSettings.GetDSPBufferSize(out dspBufferSize, out dspNumBuffers);

            m_AudioSource.timeSamples = (Microphone.GetPosition(m_Device) + AudioSettings.outputSampleRate - 3 * dspBufferSize * dspNumBuffers) % AudioSettings.outputSampleRate;
        }

        public void StopRecording()
        {
            if (string.IsNullOrEmpty(m_Device) || !Microphone.IsRecording(m_Device))
            {
                Debug.Log("No Microphone found!");
                return;
            }

            Microphone.End(m_Device);
            m_AudioSource.Stop();
        }

        public float[] GetSamples()
        {
            m_AudioSource.GetOutputData(m_Samples, 0);
            return m_Samples;
        }

        public void FillSamples(ref float[] samples)
        {
            m_AudioSource.GetOutputData(samples, 0);
        }
    }
}


