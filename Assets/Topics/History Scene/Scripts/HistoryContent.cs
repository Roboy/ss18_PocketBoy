using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.HistoryScene
{
    [CreateAssetMenu(fileName = "HistoryContent", menuName = "Pocketboy/HistoryScene/Content")]
    public class HistoryContent : ScriptableObject
    {
        public UnityEngine.Object TVContent;
        public string Date;
        [TextArea]
        public string Text;
    }
}



