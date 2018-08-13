using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.QuizSystem
{
    public class QuizResult : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> Stars = new List<GameObject>();

        private void Start()
        {
            QuizEvents.OnQuizFinished += ShowResult;
            Reset();
        }

        private void OnDestroy()
        {
            QuizEvents.OnQuizFinished -= ShowResult;
        }

        public void ShowResult(int points)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < Stars.Count && i < points; i++)
            {
                Stars[i].SetActive(true);
            }
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            foreach (var star in Stars)
            {
                star.SetActive(false);
            }
        }
    }
}
