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
        private TextMeshProUGUI CountdownText;

        private bool m_IsShowing;

        public void StartCountdown(float time, Action callback)
        {
            if (m_IsShowing)
                return;

            StartCoroutine(CountdownRoutine(time, callback));
        }

        private IEnumerator CountdownRoutine(float time, Action callback)
        {
            m_IsShowing = true;
            CountdownText.gameObject.SetActive(true);
            CountdownText.text = time.ToString("n1");
            float currentTime = time;
            while (currentTime > 0f)
            {
                CountdownText.text = currentTime.ToString("n1");
                currentTime -= Time.deltaTime;
                yield return null;
            }
            CountdownText.gameObject.SetActive(false);
            m_IsShowing = false;
            callback();
        }
    }
}


