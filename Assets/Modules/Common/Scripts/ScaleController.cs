using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class ScaleController : MonoBehaviour
    {
        [SerializeField]
        private float ScaleFactor = 2f;

        private Vector3 m_OriginalScale;

        private Coroutine m_CurrentScaleAnimation;

        private void Awake()
        {
            m_OriginalScale = transform.localScale;
        }

        public void Scale()
        {
            if (m_CurrentScaleAnimation != null)
                StopCoroutine(m_CurrentScaleAnimation);

            StartCoroutine(ScaleAnimation(transform.localScale, m_OriginalScale * ScaleFactor));
        }

        public void ResetScale()
        {
            if (m_CurrentScaleAnimation != null)
                StopCoroutine(m_CurrentScaleAnimation);

            StartCoroutine(ScaleAnimation(transform.localScale, m_OriginalScale));
        }

        private IEnumerator ScaleAnimation(Vector3 startScale, Vector3 endScale)
        {
            float duration = 0.3f;
            float currentDuration = 0f;

            while (currentDuration < duration)
            {
                transform.localScale = Vector3.Slerp(startScale, endScale, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            transform.localScale = endScale;
        }
    }
}