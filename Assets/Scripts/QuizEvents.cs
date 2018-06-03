using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.QuizSystem
{
    public class QuizEvents
    {
        public delegate void TimeOutHandler();
        public static event TimeOutHandler OnTimeOut;

        public delegate void AnswerCorrectHandler(int correctAnswer);
        public static event AnswerCorrectHandler OnAnswerCorrect;

        public delegate void AnswerIncorrectHandler(int correctAnswer, int incorrectAnswer);
        public static event AnswerIncorrectHandler OnAnswerIncorrect;

        public delegate void QuizFinishedHandler();
        public static event QuizFinishedHandler OnQuizFinished;

        public static void TimeOutEvent()
        {
            if (OnTimeOut != null)
                OnTimeOut();
        }

        public static void AnswerCorrectEvent(int correctAnswer)
        {
            if (OnAnswerCorrect != null)
                OnAnswerCorrect(correctAnswer);
        }

        public static void AnswerIncorrectEvent(int correctAnswer, int incorrectAnswer)
        {
            if (OnAnswerIncorrect != null)
                OnAnswerIncorrect(correctAnswer, incorrectAnswer);
        }

        public static void QuizFinishedEvent()
        {
            if (OnQuizFinished != null)
                OnQuizFinished();
        }
    }
}


