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

        [SerializeField]
        private Button HomeButton;

        public bool IsLoadingTriggered { get; set; }

        private bool m_IsLoading = false;

        private bool m_SavedHomeButtonState;

        private void Awake()
        {
            HomeButton.onClick.AddListener(() => LoadScene("HomeScene_DEV"));
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += (scene, mode) => FadeOutImage();
            SceneManager.sceneLoaded += (scene, mode) => ToggleHomeButton(scene);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= (scene, mode) => FadeOutImage();
            SceneManager.sceneLoaded -= (scene, mode) => ToggleHomeButton(scene);
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

        public void ShowUI()
        {
            HomeButton.gameObject.SetActive(m_SavedHomeButtonState);
        }

        public void HideUI()
        {
            m_SavedHomeButtonState = HomeButton.gameObject.activeSelf;
            HomeButton.gameObject.SetActive(false);
        }

        void FadeOutImage()
        {
            StartCoroutine(FadeImageInternal(1f, 0f, 0.5f));
        }

        void ToggleHomeButton(Scene scene)
        {
            if (scene.name == "HomeScene_DEV" || scene.name == "BaseScene")
            {
                HomeButton.gameObject.SetActive(false);
            }
            else
            {
                HomeButton.gameObject.SetActive(true);
            }
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
            HomeButton.gameObject.SetActive(false);

            AO.allowSceneActivation = true;
            m_IsLoading = false;
            IsLoadingTriggered = false;
        }
    }
}