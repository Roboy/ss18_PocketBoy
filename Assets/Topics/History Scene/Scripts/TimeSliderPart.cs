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

        private enum FadeState
        {
            FadedOut,
            FadedOutHalf,
            FadedInHalf,
            FadedIn
        }

        private FadeState State = FadeState.FadedOut;

        private TVController m_TV;

        private float m_PosToFade;

        private void Update()
        {
            // out of the mask
            if (transform.localPosition.x < -m_PosToFade || transform.localPosition.x > m_PosToFade)
            {
                FadeOutDate();
                return;
            }
            // inside the mask
            else if (transform.localPosition.x > -m_PosToFade && transform.localPosition.x < m_PosToFade)
            {
                // active element
                if (transform.localPosition.x == 0f)
                {
                    FadeInDate();
                    return;
                }
                // inside the mask but not active elment
                else
                {
                    if (State == FadeState.FadedIn)
                    {
                        FadeOutDateHalf();
                        return;
                    }
                    else
                    {
                        FadeInDateHalf();
                        return;
                    }                       
                }                        
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

        private void FadeOutDate()
        {
            if (State == FadeState.FadedOut)
                return;

            State = FadeState.FadedOut;
            StartCoroutine(Fade(0.5f, 0f));           
            Debug.Log("FadeOutDate");
        }

        private void FadeInDate()
        {
            if (State == FadeState.FadedIn)
                return;

            State = FadeState.FadedIn;
            StartCoroutine(Fade(0.5f, 1f));
            Debug.Log("FadeInDate");
        }

        private void FadeInDateHalf()
        {
            if (State == FadeState.FadedInHalf)
                return;

            State = FadeState.FadedInHalf;
            StartCoroutine(Fade(0f, 0.5f));
            Debug.Log("FadeInDateHalf");
        }

        private void FadeOutDateHalf()
        {
            if (State == FadeState.FadedOutHalf)
                return;

            State = FadeState.FadedOutHalf;
            StartCoroutine(Fade(1f, 0.5f));
            Debug.Log("FadeOutDateHalf");
        }

        private IEnumerator Fade(float startValue, float endValue)
        {
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
