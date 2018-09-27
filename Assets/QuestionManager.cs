using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using TMPro;
namespace Pocketboy.Runaround
{

    public class QuestionManager : Singleton<QuestionManager>
    {
        public List<RunaroundQuestion> Questions;
        public GameObject QuestionArea;
        public GameObject AnswerArea;
        public GameObject Buttons;
        public Sprite QuestionDefaultImage;
        public SpriteRenderer[] answer_sprites;
        private TextMeshPro[] answer_texts;
        private RunaroundQuestion m_currentQuestion;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ButtonTouch();
            }
        }
        public RunaroundQuestion GetCurrentQuestion()
        {
            return m_currentQuestion;
        }

        public void LoadQuestion(int index)
        {
            m_currentQuestion = Questions[index];
            QuestionArea.GetComponentInChildren<TextMeshPro>().text = m_currentQuestion.QuestionText;

            //Try to load image
            if (m_currentQuestion.QuestionImage != null)
            {
                QuestionArea.GetComponent<SpriteRenderer>().sprite = m_currentQuestion.QuestionImage;
            }
            else
            {
                QuestionArea.GetComponent<SpriteRenderer>().sprite = QuestionDefaultImage;
            }

            answer_sprites = AnswerArea.GetComponentsInChildren<SpriteRenderer>();
            answer_texts = AnswerArea.GetComponentsInChildren<TextMeshPro>();
            for (int i = 0; i < Questions.Count; i++)
            {
                answer_sprites[i].material = GameMaster.Instance.dic_mat_posts[i];
                answer_texts[i].text = m_currentQuestion.QuestionAnswers[i];
            }
        }

        public void NextQuestion()
        {
            int index = Questions.IndexOf(m_currentQuestion);
            if (index == (Questions.Count - 1))
            {
                index = 0;
            }
            else
            {
                index += 1;
            }
            LoadQuestion(index);
        }
        public void PreviousQuestion()
        {
            int index = Questions.IndexOf(m_currentQuestion);
            if (index == 0)
            {
                index = (Questions.Count - 1);
            }
            else
            {
                index -= 1;
            }
            LoadQuestion(index);
        }

        private void ButtonTouch()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject m_hittedObject = null;

            int layer_mask = LayerMask.GetMask("QuestionButtons");


            //Check if an object is hit at all, but only on the specific layer
            if (Physics.Raycast(ray, out hit, 1000,layer_mask))
            {
                m_hittedObject = hit.collider.gameObject;

            }

            if (m_hittedObject == null)
                return;

            if (m_hittedObject.tag == "NextQuestion")
            {
                //TODO: check for timer;
                NextQuestion();
            }

            if (m_hittedObject.tag == "PreviousQuestion")
            {
                //TODO: check for timer;
                PreviousQuestion();
            }

        }
    }

   
}
