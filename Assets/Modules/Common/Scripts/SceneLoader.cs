using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pocketboy.Common
{
    public class SceneLoader : Singleton<SceneLoader>
    {

        private bool m_IsLoading = false;

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

        IEnumerator LoadSceneAsync(string sceneName)
        {
            m_IsLoading = true;
            AsyncOperation AO = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            AO.allowSceneActivation = false;
            while (AO.progress < 0.9f)
            {
                yield return null;
            }
            AO.allowSceneActivation = true;
            m_IsLoading = false;
        }
    }
}