using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Pocketboy.SpeechPlugin
{
    public class TextToSpeechController : MonoBehaviour
    {

        [SerializeField]
        private Button PromptSpeechOutput;

        [SerializeField]
        private Text SpeechInput;

        private AndroidJavaClass m_TextToSpeechJavaClass;

        private bool m_Initialized;

        // Use this for initialization
        void Start()
        {
            Initialize();
            PromptSpeechOutput.onClick.AddListener(TextToSpeechNative);
        }

        private void Initialize()
        {
            try
            {
                m_TextToSpeechJavaClass = new AndroidJavaClass(SpeechPlugin.PACKAGE_NAME + "." + SpeechPlugin.TEXT_TO_SPEECH_CLASS);
                m_TextToSpeechJavaClass.CallStatic("setPitch", 1f);
                m_TextToSpeechJavaClass.CallStatic("setSpeed", 1f);
                m_Initialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                m_Initialized = false;
            }
        }

        void TextToSpeechNative()
        {
            if (!m_Initialized)
                return;

            m_TextToSpeechJavaClass.CallStatic("promptSpeechOutput", SpeechInput.text);
        }
    }
}