using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class RotateFade : MonoBehaviour
    {
        private bool m_Animating;

        private float m_Duration = 1f;

        private float m_MaxRotationSpeed = 2500f;

        private Renderer m_Renderer;

        private float m_StartAngle;

        private void Awake()
        {
            m_Renderer = GetComponent<Renderer>();
            m_Renderer.material = new Material(m_Renderer.sharedMaterial);
            m_StartAngle = transform.eulerAngles.y;
        }

        private void OnEnable()
        {
            m_Animating = false;
        }

        public void FadeIn()
        {
            StartCoroutine(Fade(true));
        }

        public void FadeOut()
        {
            StartCoroutine(Fade(false));
        }

        public IEnumerator FadeInWithWait()
        {
            yield return StartCoroutine(Fade(true));
        }

        public IEnumerator FadeOutWithWait()
        {
            yield return StartCoroutine(Fade(false));
        }

        private IEnumerator Fade(bool fadeIn)
        {
            if (m_Animating)
                yield break;

            m_Animating = true;
            float currentDuration = 0f;
            Color fadeColor = m_Renderer.material.color;
            float lerpFactor = 0f;

            float startLerpValue = 0f;
            float endLerpValue = 1f;
            if (fadeIn)
            {
                startLerpValue = 1f;
                endLerpValue = 0f;
            }
         
            while (currentDuration < m_Duration)
            {
                lerpFactor = Mathf.Lerp(startLerpValue, endLerpValue, currentDuration / m_Duration);
                // fading
                fadeColor.a = 1f - lerpFactor;
                m_Renderer.material.color = fadeColor;
                if (fadeIn)
                {
                    transform.Rotate(Vector3.up * Time.deltaTime * Mathf.Max(0.1f, lerpFactor) * m_MaxRotationSpeed);
                }                 
                else
                {
                    transform.Rotate(Vector3.up * Time.deltaTime * lerpFactor * m_MaxRotationSpeed);
                }
                currentDuration += Time.deltaTime;
                yield return null;
            }
            fadeColor.a = 1f - endLerpValue;
            m_Renderer.material.color = fadeColor;

            if (fadeIn)
            {
                float angleDelta = Mathf.Abs(transform.rotation.eulerAngles.y - m_StartAngle);
                float currentAngleDelta = angleDelta;
                while (currentAngleDelta > 5f)
                {
                    transform.Rotate(Vector3.up * Time.deltaTime *  0.1f * m_MaxRotationSpeed);
                    currentAngleDelta = Mathf.Abs(transform.rotation.eulerAngles.y - m_StartAngle);
                    yield return null;
                }
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, m_StartAngle, transform.eulerAngles.z);
                
            }
            m_Animating = false;
        }
    }
}
