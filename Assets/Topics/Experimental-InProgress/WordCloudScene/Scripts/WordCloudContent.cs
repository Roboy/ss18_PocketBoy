using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Wordcloud
{


    [CreateAssetMenu(fileName = "Word", menuName = "WordCloud/Words", order = 1)]
    public class WordCloudContent : ScriptableObject
    {
        public List<WordInCloud> Words;

    }

   
}
