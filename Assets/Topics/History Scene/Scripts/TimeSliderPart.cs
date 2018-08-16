using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;

namespace Pocketboy.HistoryScene
{
    public class TimeSliderPart : MonoBehaviour
    {
        [SerializeField]
        private SoftMaskScript SoftMask;

        [SerializeField]
        private Text Date;

        private float m_FloatTime = 0.2f;

        private bool m_FadedOut;

        private TVController m_TV;

        private float m_PosToFade;

        private void Update()
        {
            if (transform.localPosition.x < -m_PosToFade || transform.localPosition.x > m_PosToFade)
            {
                FadeOutDate();
            }
            else
            {
                FadeInDate();
            }
        }

        public void Setup(TimeSlider slider, RectTransform container, Vector2 position, Vector2 size, int date, Color color)
        {
            SoftMask.MaskScalingRect = container;
            transform.localPosition = position;
            GetComponent<RectTransform>().sizeDelta = size;
            Date.text = date.ToString();
            Date.transform.localPosition = new Vector2(0f, -size.y / 1.5f); // position above image
            Date.GetComponent<RectTransform>().sizeDelta = size;
            GetComponent<Image>().color = color;
            m_PosToFade = ((slider.MaxDates / 2) * slider.PartSize);
        }

        public void FadeOutDate()
        {
            if(!m_FadedOut)
                StartCoroutine(Fade(1f, 0f));
        }

        public void FadeInDate()
        {
            if (m_FadedOut)
                StartCoroutine(Fade(0f, 1f));
        }

        private IEnumerator Fade(float startValue, float endValue)
        {
            m_FadedOut = endValue == 0f ? true : false;
            float currentTime = 0f;
            Color color = Date.color;
            color.a = startValue;
            while (currentTime < m_FloatTime)
            {
                color.a = Mathf.Lerp(startValue, endValue, currentTime / m_FloatTime);
                Date.color = color;
                currentTime += Time.deltaTime;
                yield return null;
            }
            color.a = endValue;
            Date.color = color;
        }
    }
}
