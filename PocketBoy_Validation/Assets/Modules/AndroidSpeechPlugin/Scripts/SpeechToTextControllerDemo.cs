using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.SpeechPlugin
{
    public class SpeechToTextControllerDemo : MonoBehaviour
    {

        [SerializeField]
        private Button PromptSpeechInput;

        [SerializeField]
        private Text SpeechOutput;

        private AndroidJavaClass m_SpeechToTextJavaClass;

        private bool m_Initialized;

        private string m_Delimiter = ",";

        private void Start()
        {
            Initialize();
            PromptSpeechInput.onClick.AddListener(SpeechToTextNative);
        }

        private void Initialize()
        {
            try
            {
                m_SpeechToTextJavaClass = new AndroidJavaClass(SpeechPlugin.PACKAGE_NAME + "." + SpeechPlugin.SPEECH_TO_TEXT_CLASS);
                m_SpeechToTextJavaClass.CallStatic("setDelimiter", m_Delimiter);
                m_Initialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                m_Initialized = false;
            }

        }

        void SpeechToTextNative()
        {
            if (!m_Initialized)
                return;

            m_SpeechToTextJavaClass.CallStatic("startListening", gameObject.name, "OnResults");
        }

        void OnResults(string recognizedText)
        {
            SpeechOutput.text = recognizedText.Split(m_Delimiter[0])[0];
        }
    }
}

