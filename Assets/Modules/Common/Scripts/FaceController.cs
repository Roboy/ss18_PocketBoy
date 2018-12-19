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

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_Talking = false;

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
            if (m_Animator == null)
            {
                m_Animator = GameObject.FindGameObjectWithTag("FaceAnimator").GetComponent<Animator>();
            }
            #region displaying objects
            if (emotion == "cool")
            {
                m_Animator.SetBool("dealwithit", true);
            }
            #endregion
            #region looking in a direction
            if (emotion == "lookleft")
            {
                m_Animator.SetTrigger("lookleft");
            }

            if (emotion == "lookright")
            {
                m_Animator.SetTrigger("lookright");
            }

            if (emotion == "rolling")
            {
                m_Animator.SetTrigger("rolling");
            }
            #endregion
            #region eyes only
            if (emotion == "wink")
            {
                m_Animator.SetTrigger("smileblink");
            }

            if (emotion == "hypno")
            {
                m_Animator.SetTrigger("hypno_eyes");
            }

            if (emotion == "hearts")
            {
                m_Animator.SetTrigger("hearts");
            }

            #endregion



            if (emotion == "shy")
            {
                m_Animator.SetTrigger("shy");
            }

            if (emotion == "angry")
            {
                m_Animator.SetTrigger("angry");
            }

            if(emotion == "remove")
            {

                m_Animator.SetBool("dealwithit", false);
            }
        }
    }
}
