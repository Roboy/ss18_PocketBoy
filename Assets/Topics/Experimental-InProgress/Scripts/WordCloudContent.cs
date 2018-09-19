using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pocketboy.Wordcloud
{
    [CreateAssetMenu(fileName = "Word", menuName = "WordCloud/Word", order = 1)]
    public class WordCloudContent : ScriptableObject
    {
        public string Word;
        public string Explanation;
        public bool ContextRelated = false;

    }
}
