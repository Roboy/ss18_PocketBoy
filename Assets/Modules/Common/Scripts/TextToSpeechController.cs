using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pocketboy.Common
{
    public class TextToSpeechController : MonoBehaviour
    {
        public bool IsTalking { get; private set; }

        [SerializeField, HideInInspector]
        private RoboyManager m_RoboyController;

        [SerializeField, HideInInspector]
        private AndroidJavaClass m_TextToSpeechJavaClass;

        [SerializeField, HideInInspector]
        private bool m_Initialized;

        private void Awake()
        {
            Initialize();
        }

        public void Talk(string text)
        {
            if (IsTalking || !m_Initialized)
                return;

            IsTalking = true;
            m_TextToSpeechJavaClass.CallStatic("promptSpeechOutputWithCallback", text, gameObject.name, "TalkDoneInternal");
        }

        public void StopTalking()
        {
            if (!IsTalking || !m_Initialized)
                return;

            IsTalking = false;
            m_TextToSpeechJavaClass.CallStatic("stopSpeech");
        }

        private void TalkDoneInternal()
        {            
            IsTalking = false;
            m_RoboyController.TalkDone();
        }

        private void Initialize()
        {
            try
            {
                m_TextToSpeechJavaClass = new AndroidJavaClass(Pocketboy.SpeechPlugin.SpeechPlugin.PACKAGE_NAME + "." + Pocketboy.SpeechPlugin.SpeechPlugin.TEXT_TO_SPEECH_CLASS);
                m_TextToSpeechJavaClass.CallStatic("setPitch", 1f);
                m_TextToSpeechJavaClass.CallStatic("setSpeed", 1f);
                m_RoboyController = GetComponent<RoboyManager>();               
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                m_Initialized = false;
                return;
            }

            if (m_RoboyController == null)
            {
                Debug.LogError("Could not find <RoboyController>.");
                m_Initialized = false;
            }
            else
            {
                m_Initialized = true;
            }
        }
    }
}


