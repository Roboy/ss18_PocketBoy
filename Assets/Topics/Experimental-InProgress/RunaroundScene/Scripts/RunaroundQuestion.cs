﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.Runaround
{
    [CreateAssetMenu(fileName = "Question", menuName = "Pocketboy/Runaround/Question", order = 1)]
    public class RunaroundQuestion : ScriptableObject
    {
        public string QuestionText;
        public List<string> QuestionAnswers;
        public int CorrectAnswerID;
        public Sprite QuestionImage;
        public bool AnswerImagesPresent;
        public Sprite[] AnswerImages;

    }
}
