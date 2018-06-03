using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.QuizSystem
{
    [RequireComponent(typeof(Slider))]
    public class QuizTimer : MonoBehaviour
    {
        private Slider m_Slider;

        private float m_MaxTime;

        private bool m_Running;

        private float m_CurrentTime = 0f;

        private void Awake()
        {
            m_Slider = GetComponent<Slider>();
        }

        private void Update()
        {
            if (!m_Running)
                return;

            if (m_CurrentTime >= m_MaxTime)
            {
                StopTimer();
                m_Slider.value = 1f;
                QuizEvents.TimeOutEvent();
                return;
            }

            m_Slider.value = m_CurrentTime / m_MaxTime;
            m_CurrentTime += Time.unscaledDeltaTime;
        }

        public void SetupTimer(float time)
        {
            m_MaxTime = time;
            m_Slider.value = 0f;
        }

        public void StartTimer()
        {
            m_Running = true;
            m_CurrentTime = 0f;
            m_Slider.value = 0f;
            Debug.Log("Starting Time");
        }

        public void StopTimer()
        {
            m_Running = false;
        }
    }
}
