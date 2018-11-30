using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class FaceController : Singleton<FaceController>
    {

        public Animator m_Animator;

        private bool m_MouthAnimation;
        private bool m_Talking;
        private bool m_GlassesOn;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_Talking = false;
            m_GlassesOn = false;

        }

        public void StartTalkAnimation()
        {
            if (m_MouthAnimation)
                return;

            if (m_Animator == null)
            {
                m_Animator = GameObject.FindGameObjectWithTag("FaceAnimator").GetComponent<Animator>();
            }

            m_MouthAnimation = true;
            m_Animator.SetBool("talking", true);
        }

        public void StopTalkAnimation()
        {
            if (m_Animator == null)
            {
                m_Animator = GameObject.FindGameObjectWithTag("FaceAnimator").GetComponent<Animator>();
            }

            m_MouthAnimation = false;
            m_Animator.SetBool("talking", false);
        }

        public void DisplayEmotion(string emotion)
        {
            if (emotion == "dealwithit")
            {
                m_GlassesOn = !m_GlassesOn;
                m_Animator.SetBool("dealwithit", m_GlassesOn);
            }

        }
    }
}
