using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Utilities;

namespace Pocketboy.QuizSystem
{
    public class QuizManager : Singleton<QuizManager>
    {
        public QuizController CurrentQuiz { get; private set; }

        [SerializeField]
        private QuizController QuizPrefab;

        [SerializeField]
        private QuizCollection m_QuizCollection;

        [SerializeField]
        private float TimeForEachQuestion;

        private void Start()
        {
            CreateQuiz();
            //QuizEvents.OnQuizFinished += DestroyCurrentQuiz;
        }

        private void OnDisable()
        {
            //QuizEvents.OnQuizFinished -= DestroyCurrentQuiz;
        }

        public void CreateQuiz()
        {
            CurrentQuiz = Instantiate(QuizPrefab);
            CurrentQuiz.SetupQuiz(m_QuizCollection.GetQuizQuestions(), TimeForEachQuestion);
        }

        private void DestroyCurrentQuiz(int ignore)
        {
            if (CurrentQuiz != null)
                Destroy(CurrentQuiz.gameObject);
        }
    }
}