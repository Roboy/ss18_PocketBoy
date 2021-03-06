﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pocketboy.Wordcloud
{
    [CreateAssetMenu(fileName = "Word", menuName = "Pocketboy/WordCloud/Word")]
    public class WordInCloud : ScriptableObject
    {

        public string Word;
        public string Explanation;
        [HideInInspector]
        public enum CR { yes, no, undefined };
        public CR ContentRelated;

    }
}
