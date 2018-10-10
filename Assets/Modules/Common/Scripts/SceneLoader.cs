using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Pocketboy.Common
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField]
        private Image FadeImage;

        private bool m_IsLoading = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += (scene, mode) => FadeOutImage();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= (scene, mode) => FadeOutImage();
        }

        /// <summary>
        /// Loads a scene via an async operation.
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            if (m_IsLoading)
                return;

            StartCoroutine(LoadSceneAsync(sceneName));
        }

        void FadeOutImage()
        {
            StartCoroutine(FadeImageInternal(1f, 0f, 0.5f));
        }

        IEnumerator FadeImageInternal(float startValue, float endValue, float duration)
        {
            float currentDuration = 0f;
            Color fadeColor = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, startValue);
            while (currentDuration < duration)
            {
                fadeColor.a = Mathf.Lerp(startValue, endValue, currentDuration / duration);
                FadeImage.color = fadeColor;
                currentDuration += Time.deltaTime;
                yield return null;
            }
            fadeColor.a = endValue;
            FadeImage.color = fadeColor;
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            m_IsLoading = true;
            AsyncOperation AO = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            AO.allowSceneActivation = false;
            while (AO.progress < 0.9f)
            {
                yield return null;
            }

            yield return StartCoroutine(FadeImageInternal(0f, 1f, 0.5f));

            AO.allowSceneActivation = true;
            m_IsLoading = false;
        }
    }
}