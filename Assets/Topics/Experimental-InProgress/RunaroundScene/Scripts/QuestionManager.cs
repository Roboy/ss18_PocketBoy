﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using TMPro;
using UnityEngine.UI;

namespace Pocketboy.Runaround
{

    public class QuestionManager : Singleton<QuestionManager>
    {
        public GameObject AnswerArea;
        public List<RunaroundQuestion> Questions;
        public Image QuestionImage;
        public Sprite QuestionDefaultSprite;
        public Material mat_QuestionTransparent;
        public Material mat_QuestionSolid;
        public Image[] answer_Images;
        private RunaroundQuestion m_currentQuestion = null;

        public void NavigateQuestion(string tag)
        {
            if (tag == "NextQuestion")
            {
                NextQuestion();
            }
            if (tag == "PreviousQuestion")
            {
                PreviousQuestion();
            }
        }

        public RunaroundQuestion GetCurrentQuestion()
        {
            return m_currentQuestion;
        }

        public void ToggleAnswersVisibility(string operation)
        {
            if (operation == "ON")
            {
                AnswerArea.SetActive(true);
            }

            if (operation == "OFF")
            {
                AnswerArea.SetActive(false);
            }


        }

        public void LoadQuestion(int index)
        {
            ToggleAnswersVisibility("OFF");
            GameMaster.Instance.ToggleHUDvisibility("OFF");
            GameMaster.Instance.ToggleResultText("OFF");
            m_currentQuestion = Questions[index];
            StartCoroutine(GameMaster.Instance.ResetGame());
            QuestionImage.GetComponentInChildren<TextMeshProUGUI>().text = m_currentQuestion.QuestionText;
            RoboyManager.Instance.Talk(m_currentQuestion.QuestionText);

            //Try to load image for question.
            if (m_currentQuestion.QuestionImage != null)
            {
                QuestionImage.sprite = m_currentQuestion.QuestionImage;
                QuestionImage.material = mat_QuestionSolid;
            }
            else
            {
                QuestionImage.sprite = QuestionDefaultSprite;
                QuestionImage.material = mat_QuestionTransparent;
            }
            //Assign the image and text components of the current question regarding answers.
            for (int i = 0; i < answer_Images.Length; i++)
            {
                //Assign the same material as the corresponding floor.
                answer_Images[i].material = GameMaster.Instance.dic_mat_floors[i];
                answer_Images[i].material.shader = Shader.Find("UI/Unlit/Transparent");
                answer_Images[i].GetComponentInChildren<TextMeshProUGUI>().text = m_currentQuestion.QuestionAnswers[i];

                //Try to load image for every single answer.
                if (!m_currentQuestion.AnswerImagesPresent)
                {
                    answer_Images[i].sprite = QuestionDefaultSprite;

                }
                else
                {
                    answer_Images[i].sprite = m_currentQuestion.AnswerImages[i];
                }
                

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

    }


}
