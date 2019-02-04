using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Pocketboy.Common
{
    public class CountdownManager : Singleton<CountdownManager>
    {
        [SerializeField]
        private TextMeshProUGUI BlockingCountdownText;

        [SerializeField]
        private TextMeshProUGUI WarningCountdownText;

        private bool m_IsShowing;

        private TextMeshProUGUI m_CurrentCountdownText;

        private float m_CurrentCountdownTime;

        private Action m_CurrentCallback;

        private void Update()
        {
            UpdateCountdown();
        }

        public void StartCountdownBlockingInput(float time, Action callback)
        {
            if (m_IsShowing)
                return;

            StartCountdown(BlockingCountdownText, time, callback);
        }

        public void StartWarningCountdown(float time, Action callback)
        {
            if (m_IsShowing)
                return;

            StartCountdown(WarningCountdownText, time, callback);
        }

        public void StopCountdown()
        {
            m_CurrentCountdownText.gameObject.SetActive(false);
            m_IsShowing = false;
        }

        private void StartCountdown(TextMeshProUGUI text, float time, Action callback)
        {
            text.gameObject.SetActive(true);
            text.text = time.ToString("n1");
            m_CurrentCallback = callback;
            m_CurrentCountdownText = text;
            m_CurrentCountdownTime = time;
            m_IsShowing = true;
        }

        private void UpdateCountdown()
        {
            if (!m_IsShowing)
                return;

            m_CurrentCountdownText.text = m_CurrentCountdownTime.ToString("n1");
            m_CurrentCountdownTime -= Time.deltaTime;

            if (m_CurrentCountdownTime < 0f)
            {
                m_CurrentCountdownText.gameObject.SetActive(false);
                m_IsShowing = false;
                m_CurrentCallback();
            }
        }
    }
}


