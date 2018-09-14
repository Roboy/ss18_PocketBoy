using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.Common
{
    public class SpeechToTextController : MonoBehaviour
    {
        public bool IsListening { get; private set; }

        [SerializeField]
        private Button ListenButton;

        [SerializeField, HideInInspector]
        private RoboyManager m_RoboyManager;

        [SerializeField, HideInInspector]
        private AndroidJavaClass m_SpeechToTextJavaClass; 

        private string m_Delimiter = ",";

        private Color m_InitColor;

        [SerializeField, HideInInspector]
        private bool m_Initialized;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                m_SpeechToTextJavaClass = new AndroidJavaClass(Pocketboy.SpeechPlugin.SpeechPlugin.PACKAGE_NAME + "." + Pocketboy.SpeechPlugin.SpeechPlugin.SPEECH_TO_TEXT_CLASS);
                m_SpeechToTextJavaClass.CallStatic("setDelimiter", m_Delimiter);
                ListenButton.onClick.AddListener(HandleListening);
                m_InitColor = ListenButton.image.color;
                m_RoboyManager = RoboyManager.Instance;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                m_Initialized = false;
                return;
            }

            if (m_RoboyManager == null)
            {
                Debug.LogError("Could not find <RoboyManager>.");
                m_Initialized = false;
            }
            else
            {
                m_Initialized = true;
            }
        }

        public void StartListening()
        {
            if (!m_Initialized || IsListening)
                return;

            IsListening = true;
            ListenButton.image.color = Color.red;
            m_SpeechToTextJavaClass.CallStatic("startListening", gameObject.name, "StopListeningInternal");
        }

        public void StopListening()
        {
            if (!m_Initialized || !IsListening)
                return;

            m_SpeechToTextJavaClass.CallStatic("stopListening");
            Debug.Log("DICK: STOP LISTENING");
        }

        private void HandleListening()
        {
            if (!m_Initialized)
                return;

            if (!IsListening)
                StartListening();
            else
                StopListening();
        }

        private void StopListeningInternal(string recognizedText)
        {
            IsListening = false;
            ListenButton.image.color = m_InitColor;
            m_RoboyManager.ListenDone(recognizedText.Split(m_Delimiter[0])[0]);
            Debug.Log("DICK: STOP INTERNAL");
        }
    }
}


