using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pocketboy.SpeechPlugin;

namespace Pocketboy.Common
{
    public class TextToSpeechManager : Singleton<TextToSpeechManager>
    {
        public bool IsTalking { get; private set; }

        private AndroidJavaClass m_TextToSpeechJavaClass;

        [HideInInspector]
        public bool m_Initialized;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public void Talk(string text)
        {
            if (IsTalking || !m_Initialized)
                return;

            IsTalking = true;
            m_TextToSpeechJavaClass.CallStatic("promptSpeechOutputWithCallback", text, gameObject.name, "TalkDone");
            LevelManager.Instance.Roboy.StartTalkAnimation();
        }

        private void TalkDone()
        {
            IsTalking = false;
            LevelManager.Instance.Roboy.StopTalkAnimation();
        }

        private void Initialize()
        {
            try
            {
                m_TextToSpeechJavaClass = new AndroidJavaClass(Pocketboy.SpeechPlugin.SpeechPlugin.PACKAGE_NAME + "." + Pocketboy.SpeechPlugin.SpeechPlugin.TEXT_TO_SPEECH_CLASS);
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
    }
}


