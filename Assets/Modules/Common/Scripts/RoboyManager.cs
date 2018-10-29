using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.SceneManagement;

namespace Pocketboy.Common
{
    /// <summary>
    /// TODO:: Add functionality e.g. startAnimationXY
    /// </summary>
    public class RoboyManager : PersistentSingleton<RoboyManager>
    {
        public Anchor ARAnchor { get; private set; }

        public bool IsTalking { get { return m_TextToSpeechController.IsTalking; } }

        public bool IsListening { get { return m_SpeechToTextController.IsListening; } }

        public Vector3 InstructionPosition { get { return InstructionTransformPosition.position; } }

        [SerializeField]
        private Transform Head;

        [SerializeField]
        private Transform InstructionTransformPosition;

        private Coroutine m_MouthAnimation;

        private TextToSpeechController m_TextToSpeechController;

        private SpeechToTextController m_SpeechToTextController;

        private FaceController m_FaceController;

        private bool m_Initialized = false;

        private void Start()
        {
            if(!m_Initialized)
                LookAtCamera();

            // roboy should stop talking and listening when loading a new scene, we cannot use sceneLoaded event cause some scenes may trigger roboy talking at the start of the scene
            SceneManager.sceneUnloaded += (scene) => StopTalking();
            SceneManager.sceneUnloaded += (scene) => StopListening();
        }

        void Update()
        {
            Head.LookAt(Camera.main.transform.position);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                // Application is focused here
            }
            else
            {
                StopTalking();
                StopListening();
            }
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
            if (string.IsNullOrEmpty(text))
                return;

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
            m_FaceController.StopTalkAnimation();
        }

        public void Listen()
        {
            m_SpeechToTextController.StartListening();
        }

        public void ListenDone(string recognizedText)
        {
            //if (recognizedText.Contains("home") || recognizedText.Contains("beam") || recognizedText.Contains("scotty"))
            //{
            //    SceneLoader.Instance.LoadScene("HomeScene");
            //}
        }

        public void StopListening()
        {
            m_SpeechToTextController.StopListening();
        }

        private void LookAtCamera()
        {
            var cameraPosition = Camera.main.transform.position;
            cameraPosition.y = transform.position.y;
            transform.LookAt(cameraPosition);
        }
    }
}


