using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Utilities;

namespace Pocketboy.QuizSystem
{
    public class QuizController : MonoBehaviour
    {
        [SerializeField]
        private QuizUIController UIController;

        [SerializeField]
        private float AnswerShowTime = 2f;

        public int CurrentQuestion { get; private set; }

        private List<QuizQuestion> m_QuizQuestions = new List<QuizQuestion>();

        private float m_TimeForEachQuestion;

        private int m_Points = 0;

        private void Start()
        {
            QuizEvents.OnTimeOut += TimeOut;
        }

        private void OnDestroy()
        {
            QuizEvents.OnTimeOut -= TimeOut;
        }

        public void SetupQuiz(List<QuizQuestion> quizQuestions, float timeForEachQuestion)
        {
            m_QuizQuestions = quizQuestions;
            UIController.SetupUI(quizQuestions.Count, timeForEachQuestion);
            CurrentQuestion = 0;
            m_Points = 0;
            UIController.SetupUIForQuestion(m_QuizQuestions[CurrentQuestion]);
        }

        public void SubmitAnswer(int answerNumber)
        {
            bool isCorrect = answerNumber == m_QuizQuestions[CurrentQuestion].CorrectAnswer;
            if (isCorrect)
            {
                QuizEvents.AnswerCorrectEvent(answerNumber);
                m_Points++;
            }
            else
            {
                QuizEvents.AnswerIncorrectEvent(m_QuizQuestions[CurrentQuestion].CorrectAnswer, answerNumber);
            }
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            StartCoroutine(ShowNextQuestionCoroutine());
                     
        }

        private void TimeOut()
        {
            SubmitAnswer(-1);
        }

        private IEnumerator ShowNextQuestionCoroutine()
        {
            yield return new WaitForSeconds(AnswerShowTime);
            CurrentQuestion++;
            if (CurrentQuestion > m_QuizQuestions.Count - 1)
            {
                QuizEvents.QuizFinishedEvent(m_Points);
            }
            else
            {
                UIController.SetupUIForQuestion(m_QuizQuestions[CurrentQuestion]);
            }
        }
    }
}