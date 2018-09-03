using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Pocketboy.Common
{
    public class SpeechToTextManager :  Singleton<SpeechToTextManager>
    {
        [SerializeField]
        private Button ListenButton;

        private AndroidJavaClass m_SpeechToTextJavaClass;

        [HideInInspector]
        public bool m_Initialized;

        private string m_Delimiter = ",";

        [HideInInspector]
        public bool m_Listening = false;

        private Color m_InitColor;

        protected override void Awake()
        {
            base.Awake();
            Initialize();           
            m_InitColor = ListenButton.image.color;
        }

        private void Start()
        {
            ListenButton.onClick.AddListener(Listen);
        }

        private void Initialize()
        {
            try
            {
                m_SpeechToTextJavaClass = new AndroidJavaClass(Pocketboy.SpeechPlugin.SpeechPlugin.PACKAGE_NAME + "." + Pocketboy.SpeechPlugin.SpeechPlugin.SPEECH_TO_TEXT_CLASS);
                m_SpeechToTextJavaClass.CallStatic("setDelimiter", m_Delimiter);
                m_Initialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                m_Initialized = false;
            }
        }

        public void Listen()
        {
            if (!m_Initialized || m_Listening)
                return;

            m_Listening = true;
            ListenButton.image.color = Color.red;
            m_SpeechToTextJavaClass.CallStatic("promptSpeechInput", gameObject.name, "HandleResult");
        }

        public void HandleResult(string recognizedText)
        {
            m_Listening = false;
            ListenButton.image.color = m_InitColor;
            string userInput = recognizedText.Split(m_Delimiter[0])[0];
            if (userInput.Contains("home") || userInput.Contains("scotty") || userInput.Contains("beam"))
            {
                SceneLoadAnimator.Instance.StartAnimation("HomeScene");
            }
        }
    }
}

