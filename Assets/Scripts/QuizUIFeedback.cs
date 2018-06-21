using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Pocketboy.QuizSystem
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class QuizUIFeedback : MonoBehaviour
    {
        [SerializeField]
        private Color DefaultColor;

        [SerializeField]
        private Color CorrectColor;

        [SerializeField]
        private Color IncorrectColor;

        private HorizontalLayoutGroup m_HorizontalLayoutGroup;

        private List<Image> m_FeedbackImages = new List<Image>();

        private void Awake()
        {
            m_HorizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        }

        public void SetupFeedback(int questionCount)
        {
            var width = GetComponent<RectTransform>().sizeDelta.x;
            var height = GetComponent<RectTransform>().sizeDelta.y;

            // e.g. 1000 / 5 - 0.01 * 1000 = 190
            var widthForEachElement = width / (float) questionCount - 0.01f * width;
            // e.g. 0.01 * 1000 * 5 / 4 = 10 * 5 / 4 = 12.5
            var spacing = 0.01f * width * questionCount / (float) (questionCount - 1);
            m_HorizontalLayoutGroup.spacing = spacing;
            for (int i = 0; i < questionCount; i++)
            {
                GameObject feedbackElement = new GameObject("Feedback " + (i+1));
                var image = feedbackElement.AddComponent<Image>();
                image.color = DefaultColor;
                image.transform.parent = transform;
                image.GetComponent<RectTransform>().sizeDelta = new Vector2(widthForEachElement, height);
                m_FeedbackImages.Add(image);
            }
        }

        public void ShowCorrectFeedback()
        {
            m_FeedbackImages[QuizManager.Instance.CurrentQuiz.CurrentQuestion].color = CorrectColor;
        }

        public void ShowIncorrectFeedback()
        {
            m_FeedbackImages[QuizManager.Instance.CurrentQuiz.CurrentQuestion].color = IncorrectColor;
        }

        public void ShowResult()
        {

        }
    }
}


