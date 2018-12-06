using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.HistoryScene
{
    [CreateAssetMenu(fileName = "HistoryContent", menuName = "HistoryScene/Content", order = 1)]
    public class HistoryContent : ScriptableObject
    {
        public UnityEngine.Object TVContent;
        public string Date;
        public string Text;
    }
}



