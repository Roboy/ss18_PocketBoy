using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pocketboy.QuizSystem
{
    [Serializable]
    public class QuizQuestion
    {
        public int ID;

        public bool IsPictureBased;

        public string Question;

        public string[] Answers;

        public int CorrectAnswer;

        public bool CanBeKnown;

        public bool Asked;
    }
}


