using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

namespace Pocketboy.Common
{
    /// <summary>
    /// TODO:: Add functionality e.g. startAnimationXY, SayXY
    /// </summary>
    public class RoboyController : MonoBehaviour
    {
        public Anchor ARAnchor { get; private set; }

        [SerializeField]
        private Transform Head;

        private Coroutine m_MouthAnimation;

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

        public void StartTalkAnimation()
        {
            m_MouthAnimation = StartCoroutine(EmotionManager.Instance.mouthMoving());
        }

        public void StopTalkAnimation()
        {
            if (m_MouthAnimation != null)
                StopCoroutine(m_MouthAnimation);
            EmotionManager.Instance.ResetMouth();
        }

        public void Initialize(Anchor anchor)
        {
            if (m_Initialized)
                return;

            LookAtCamera();
            ARAnchor = anchor;
            m_Initialized = true;

            GetComponent<DontDestroyOnLoadObject>().Hack();
        }

        private void LookAtCamera()
        {
            var cameraPosition = Camera.main.transform.position;
            cameraPosition.y = transform.position.y;
            transform.LookAt(cameraPosition);
        }
    }
}


