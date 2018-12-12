using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Wordcloud
{


    [CreateAssetMenu(fileName = "Word", menuName = "Pocketboy/WordCloud/Words")]
    public class WordCloudContent : ScriptableObject
    {
        public List<WordInCloud> Words;

    }

   
}
