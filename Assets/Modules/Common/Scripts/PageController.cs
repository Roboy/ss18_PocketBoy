using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Pocketboy.Common
{
    public class PageController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI Text;
       
        public bool NextPage()
        {
            int previousPage = Text.pageToDisplay;
            Text.pageToDisplay = Mathf.Min(Text.textInfo.pageCount, Text.pageToDisplay + 1);
            return previousPage != Text.pageToDisplay;
        }

        public bool PreviousPage()
        {
            int previousPage = Text.pageToDisplay;
            Text.pageToDisplay = Mathf.Max(0, Text.pageToDisplay - 1);
            return previousPage != Text.pageToDisplay;
        }

        public bool NextPage(TMP_Text text)
        {
            int previousPage = text.pageToDisplay;
            text.pageToDisplay = Mathf.Min(text.textInfo.pageCount, text.pageToDisplay + 1);
            return previousPage != text.pageToDisplay;
        }

        public bool PreviousPage(TMP_Text text)
        {
            int previousPage = text.pageToDisplay;
            text.pageToDisplay = Mathf.Max(0, text.pageToDisplay - 1);
            return previousPage != text.pageToDisplay;
        }
    }
}


