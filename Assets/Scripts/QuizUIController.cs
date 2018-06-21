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
        private Button[] AnswerButtons;

        [SerializeField]
        private QuizUIFeedback QuizUIFeedback;

        [SerializeField]
        private QuizTimer QuizTimer;

        [SerializeField]
        private Color DefaultButtonColor;

        private Dictionary<Button, Animator> m_ButtonAnimators = new Dictionary<Button, Animator>();

        private void Awake()
        {
            CacheButtonAnimators();
        }

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
                AnswerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.Answers[i];
                AnswerButtons[i].image.color = DefaultButtonColor;
                AnswerButtons[i].interactable = true;
            }
            QuizTimer.StartTimer();
        }

        private void CorrectAnswerAnimation(int correctAnswer)
        {
            ToggleButtonInteraction(false);
            AnswerButtons[correctAnswer].image.color = Color.green;
            QuizTimer.StopTimer();
            QuizUIFeedback.ShowCorrectFeedback();
        }

        private void IncorrectAnswerAnimation(int correctAnswer, int incorrectAnswer)
        {
            ToggleButtonInteraction(false);
            StartCoroutine(ButtonBlinkAnimation(AnswerButtons[correctAnswer], Color.green));
            //AnswerButtons[correctAnswer].image.color = Color.green;
            if (incorrectAnswer != -1)
            {
                AnswerButtons[incorrectAnswer].image.color = Color.red;
                m_ButtonAnimators[AnswerButtons[incorrectAnswer]].SetTrigger("Shake");
            }
                
            QuizTimer.StopTimer();
            QuizUIFeedback.ShowIncorrectFeedback();
        }

        private void ToggleButtonInteraction(bool value)
        {
            foreach (var button in AnswerButtons)
            {
                button.interactable = value;
            }
        }

        private void CacheButtonAnimators()
        {
            foreach (var button in AnswerButtons)
            {
                var animator = button.GetComponent<Animator>();
                if (animator != null)
                    m_ButtonAnimators.Add(button, animator);
            }
        }

        private IEnumerator ButtonBlinkAnimation(Button button, Color blinkColor)
        {
            Color defaultColor = button.image.color;
            float currentTime = 0f;
            float stepTime = 0.2f;
            float animTime = 1f;
            bool isColored = false;
            while (currentTime < animTime)
            {
                if (isColored)
                {
                    button.image.color = defaultColor;
                }
                else
                {
                    button.image.color = blinkColor;
                }
                isColored = !isColored;
                currentTime += stepTime;
                yield return new WaitForSeconds(stepTime);
            }
            button.image.color = blinkColor;
        }
    }
}


