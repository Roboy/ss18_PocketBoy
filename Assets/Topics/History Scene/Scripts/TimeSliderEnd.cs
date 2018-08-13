using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.HistoryScene
{ 
    [RequireComponent(typeof(Image))]
    public class TimeSliderEnd : MonoBehaviour
    {

        public Image ImageComponent;

        public void Setup(Vector2 position, Vector2 size, Color color)
        {
            transform.localPosition = position;
            GetComponent<RectTransform>().sizeDelta = size;
            ImageComponent.color = color;
        }

        public void Show(float duration)
        {
            if (ImageComponent.enabled)
                return;

            StartCoroutine(FillAnimation(0f, 1f, duration, true));
        }

        public void Hide(float duration)
        {
            if (!ImageComponent.enabled)
                return;

            StartCoroutine(FillAnimation(1f, 0f, duration, false));
        }

        private IEnumerator FillAnimation(float startValue, float endValue, float duration, bool newState)
        {
            if (newState)
                ImageComponent.enabled = true;
            ImageComponent.fillAmount = startValue;
            float currentDuration = 0f;
            while (currentDuration < duration)
            {
                ImageComponent.fillAmount = Mathf.SmoothStep(startValue, endValue, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            ImageComponent.fillAmount = endValue;
            if (!newState)
                ImageComponent.enabled = false;
        }
    }
}