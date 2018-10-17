using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.SceneManagement;

namespace Pocketboy.Common
{
    /// <summary>
    /// TODO:: Add functionality e.g. startAnimationXY, SayXY, StopTalking, StopListening
    /// </summary>
    public class RoboyManager : PersistentSingleton<RoboyManager>
    {
        public Anchor ARAnchor { get; private set; }

        public bool IsTalking { get { return m_TextToSpeechController.IsTalking; } }

        public bool IsListening { get { return m_SpeechToTextController.IsListening; } }

        [SerializeField]
        private Transform Head;

        private Coroutine m_MouthAnimation;

        private TextToSpeechController m_TextToSpeechController;

        private SpeechToTextController m_SpeechToTextController;

        private FaceController m_FaceController;

        private bool m_Initialized = false;

        private void Start()
        {
            if(!m_Initialized)
                LookAtCamera();
        }

        void Update()
        {
            Head.LookAt(Camera.main.transform.position);
        }

        public void Initialize(Anchor anchor)
        {
            if (m_Initialized)
                return;

            m_TextToSpeechController = GetComponentInChildren<TextToSpeechController>(true);
            m_SpeechToTextController = GetComponentInChildren<SpeechToTextController>(true);
            m_FaceController = GetComponentInChildren<FaceController>(true);
            base.Awake();

            LookAtCamera();
            ARAnchor = anchor;
            m_Initialized = true;
        }

        public void Talk(string text)
        {
            m_TextToSpeechController.Talk(text);
            m_FaceController.StartTalkAnimation();
        }

        public void TalkDone()
        {
            m_FaceController.StopTalkAnimation();
        }

        public void StopTalking()
        {
            m_TextToSpeechController.StopTalking();
        }

        public void Listen()
        {
            m_SpeechToTextController.StartListening();
        }

        public void ListenDone(string recognizedText)
        {
            if (recognizedText.Contains("home") || recognizedText.Contains("beam") || recognizedText.Contains("scotty"))
            {
                SceneLoader.Instance.LoadScene("HomeScene");
            }
        }

        private void LookAtCamera()
        {
            var cameraPosition = Camera.main.transform.position;
            cameraPosition.y = transform.position.y;
            transform.LookAt(cameraPosition);
        }
    }
}


