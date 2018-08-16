﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.Common
{
    public class SceneLoadAnimator : Singleton<SceneLoadAnimator>
    {
        [SerializeField, Range(0.5f, 2f)]
        private float TriggerTime = 1f;

        [SerializeField]
        private float LogoRotationSpeed = 10f;

        [SerializeField]
        private RectTransform RoboyLogo;

        private Vector2 m_EndSize;

        private Coroutine m_AnimationCoroutine;

        private void Awake()
        {
            m_EndSize = Vector2.one * Screen.width * 1.4f;
        }

        public void StartAnimation(string sceneToLoadAfterAnimation)
        {
            m_AnimationCoroutine = StartCoroutine(RoboyHeadAnimation(sceneToLoadAfterAnimation));
        }

        public void StopAnimation()
        {
            if (m_AnimationCoroutine != null)
            {
                StopCoroutine(m_AnimationCoroutine);
            }
        }

        private IEnumerator RoboyHeadAnimation(string sceneToLoadAfterAnimation)
        {
            float currentTime = 0f;
            RoboyLogo.gameObject.SetActive(true);
            RoboyLogo.sizeDelta = Vector2.zero;
            while (currentTime < TriggerTime)
            {
                RoboyLogo.transform.Rotate(Vector3.forward, LogoRotationSpeed, Space.Self);
                RoboyLogo.sizeDelta = Vector2.Lerp(Vector2.zero, m_EndSize, currentTime / TriggerTime);
                currentTime += Time.deltaTime;
                yield return null;
            }
            RoboyLogo.sizeDelta = m_EndSize;
            SceneLoader.Instance.LoadScene(sceneToLoadAfterAnimation);
            StartCoroutine(ReverseHeadAnimation());
        }

        private IEnumerator ReverseHeadAnimation()
        {
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

