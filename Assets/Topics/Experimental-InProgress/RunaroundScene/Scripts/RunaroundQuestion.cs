using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.Runaround
{
    [CreateAssetMenu(fileName = "Question", menuName = "Runaround/Question", order = 1)]
    public class RunaroundQuestion : ScriptableObject
    {
        public string QuestionText;
        public List<string> QuestionAnswers;
        public int CorrectAnswerID;
        public Sprite QuestionImage;

    }
}
