using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pocketboy.QuizSystem
{
    public class QuizUIController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI QuestionText;

        [SerializeField]
        private QuizAnswerButton[] AnswerButtons;

        [SerializeField]
        private QuizUIFeedback QuizUIFeedback;

        [SerializeField]
        private QuizTimer QuizTimer;


        private void Start()
        {
            QuizEvents.OnAnswerCorrect += CorrectAnswerAnimation;
            QuizEvents.OnAnswerIncorrect += IncorrectAnswerAnimation;
            
        }

        private void OnDestroy()
        {
            QuizEvents.OnAnswerCorrect -= CorrectAnswerAnimation;
            QuizEvents.OnAnswerIncorrect -= IncorrectAnswerAnimation;
        }

        public void SetupUI(int questionCount, float timeForEachQuestion)
        {
            QuizUIFeedback.SetupFeedback(questionCount);
            QuizTimer.SetupTimer(timeForEachQuestion);
        }

        public void SetupUIForQuestion(QuizQuestion question)
        {
            QuestionText.text = question.Question;
            for (int i = 0; i < question.Answers.Length; i++)
            {
                if (!question.IsPictureBased)
                {
                    AnswerButtons[i].LoadText(question.Answers[i]);
                }
                else
                {
                    AnswerButtons[i].LoadImage(question.ImageAnswers[i]);
                }
            }
            QuizTimer.StartTimer();
        }

        private void CorrectAnswerAnimation(int correctAnswer)
        {
            ToggleButtonInteraction(false);
            AnswerButtons[correctAnswer].CorrectAnimation();
            QuizTimer.StopTimer();
            QuizUIFeedback.ShowCorrectFeedback();
        }

        private void IncorrectAnswerAnimation(int correctAnswer, int incorrectAnswer)
        {
            ToggleButtonInteraction(false);
            AnswerButtons[correctAnswer].CorrectAnimation();
            if (incorrectAnswer != -1)
            {
                AnswerButtons[incorrectAnswer].IncorrectAnimation();
            }
            QuizTimer.StopTimer();
            QuizUIFeedback.ShowIncorrectFeedback();
        }

        private void ToggleButtonInteraction(bool value)
        {
            foreach (var button in AnswerButtons)
            {
                button.Deactivate();
            }
        }
    }
}


