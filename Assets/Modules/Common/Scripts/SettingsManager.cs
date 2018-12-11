using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace Pocketboy.Common
{

    public class SettingsManager : Singleton<SettingsManager>
    {
        [SerializeField]
        private Button m_SettingsButton;

        [SerializeField]
        private GameObject m_AudioPanel;

        [SerializeField]
        private Slider m_SpeechSlider;

        [SerializeField]
        private Slider m_FXSlider;

        [SerializeField]
        private Slider m_MusicSlider;

        [SerializeField]
        private AudioMixer m_MasterAudioMixer;

        [SerializeField]
        private AudioSource m_AudioSpeech;

        [SerializeField]
        private AudioSource m_AudioUI;

        [SerializeField]
        private AudioSource m_AudioWorld;

        [SerializeField]
        private AudioSource m_AudioMusic;

        private void Awake()
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }

        void Start()
        {
            m_SettingsButton.onClick.AddListener(ToggleSettings);
            m_SpeechSlider.onValueChanged.AddListener(delegate { SetSpeechVolume(); });
            m_FXSlider.onValueChanged.AddListener(delegate { SetFXVolume(); });
            m_MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
            InitializeAudio();
        }

        public void InitializeAudio()
        {
            //m_MasterAudioMixer.SetFloat("VolSpeech", CalculateDB(m_SpeechSlider.value));
            //m_MasterAudioMixer.SetFloat("VolFX", CalculateDB(m_FXSlider.value));
            //m_MasterAudioMixer.SetFloat("VolMusic", CalculateDB(m_MusicSlider.value));
            m_AudioSpeech.volume = m_SpeechSlider.value;
            m_AudioUI.volume = m_FXSlider.value;
            m_AudioWorld.volume = m_FXSlider.value;
            m_AudioMusic.volume = m_MusicSlider.value;
        }

        public void ToggleSettings()
        {
            m_AudioPanel.SetActive(!m_AudioPanel.activeInHierarchy);
            AudioSourcesManager.Instance.PlaySound("ButtonClick");
        }

        public void SetSpeechVolume()
        {
            //m_MasterAudioMixer.SetFloat("VolSpeech", CalculateDB(m_SpeechSlider.value));
            m_AudioSpeech.volume = m_SpeechSlider.value;
        }

        public void SetFXVolume()
        {
            //m_MasterAudioMixer.SetFloat("VolFX", CalculateDB(m_FXSlider.value));
            m_AudioUI.volume = m_FXSlider.value;
            m_AudioWorld.volume = m_FXSlider.value;
        }

        public void SetMusicVolume()
        {
            //m_MasterAudioMixer.SetFloat("VolMusic", CalculateDB(m_MusicSlider.value));
            m_AudioMusic.volume = m_MusicSlider.value;

        }

        public float CalculateDB(float value)
        {
            float result = value * 100.0f;
            result -= 80.0f;
            return result;
        }
    }
}
