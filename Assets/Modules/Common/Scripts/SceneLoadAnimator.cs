﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Pocketboy.Common
{
    public class SceneLoadAnimator : Singleton<SceneLoadAnimator>
    {
        [SerializeField, Range(0.05f, 0.2f)]
        private float TriggerTime = 1f;

        [SerializeField]
        private float LogoRotationSpeed = 10f;

        [SerializeField]
        private RectTransform RoboyLogo;

        private Vector2 m_EndSize;

        private Coroutine m_AnimationCoroutine;

        protected void Start()
        {
            m_EndSize = Vector2.one * Screen.width * 1.4f;
        }

        //private void OnEnable()
        //{
        //    SceneManager.sceneLoaded += SceneStartAnimaton;
        //}

        //private void OnDisable()
        //{
        //    SceneManager.sceneLoaded -= SceneStartAnimaton;
        //}

        public void SceneStartAnimaton(Scene scene, LoadSceneMode mode)
        {
            StopAnimation();
            m_AnimationCoroutine = StartCoroutine(ReverseHeadAnimation());
        }

        public IEnumerator SceneEndAnimation()
        {
            StopAnimation();
            m_AnimationCoroutine = StartCoroutine(RoboyHeadAnimation());
            yield return m_AnimationCoroutine;
        }

        public void StopAnimation()
        {
            if (m_AnimationCoroutine != null)
            {
                StopCoroutine(m_AnimationCoroutine);
            }
            RoboyLogo.gameObject.SetActive(false);
            RoboyLogo.sizeDelta = Vector2.zero;
        }

        private IEnumerator RoboyHeadAnimation()
        {
            RoboyLogo.gameObject.SetActive(true);
            float currentTime = 0f;           
            RoboyLogo.sizeDelta = Vector2.zero;
            while (currentTime < TriggerTime)
            {
                RoboyLogo.transform.Rotate(Vector3.forward, LogoRotationSpeed, Space.Self);
                RoboyLogo.sizeDelta = Vector2.Lerp(Vector2.zero, m_EndSize, currentTime / TriggerTime);
                currentTime += Time.deltaTime;
                yield return null;
            }
            RoboyLogo.sizeDelta = m_EndSize;
            RoboyLogo.gameObject.SetActive(false);
            //StartCoroutine(ReverseHeadAnimation());            
        }

        private IEnumerator ReverseHeadAnimation()
        {
            RoboyLogo.gameObject.SetActive(true);
            float currentTime = 0f;           
            RoboyLogo.sizeDelta = m_EndSize;
            while (currentTime < TriggerTime)
            {
                RoboyLogo.transform.Rotate(Vector3.forward, -LogoRotationSpeed, Space.Self);
                RoboyLogo.sizeDelta = Vector2.Lerp(m_EndSize, Vector2.zero, currentTime / TriggerTime);
                currentTime += Time.deltaTime;
                yield return null;
            }
            RoboyLogo.sizeDelta = Vector2.zero;
            RoboyLogo.gameObject.SetActive(false);
        }
    }
}


