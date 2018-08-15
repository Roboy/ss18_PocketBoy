using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.HistoryScene
{
    public class TVButton : MonoBehaviour
    {
        private enum ButtonType
        {
            Next,
            Previous,
            Repeat
        }

        [SerializeField]
        private TVController TV;

        [SerializeField]
        private TimeSlider Slider;

        [SerializeField]
        private ButtonType ButtonOperation;

        [SerializeField]
        private GameObject Highlight;

        private float m_HighlightTime = 0.15f;

        private void OnMouseDown()
        {
            switch(ButtonOperation)
            {
                case ButtonType.Next:
                    TV.ShowNextContent();
                    Slider.ShowNextDate();
                    break;
                case ButtonType.Previous:
                    TV.ShowPreviousContent();
                    Slider.ShowPreviousDate();
                    break;
                case ButtonType.Repeat:
                    TV.RepeatContent();
                    break;
            }
            StartCoroutine(HighlightAnimation());
        }

        private IEnumerator HighlightAnimation()
        {
            if (Highlight == null)
                yield break ;

            Highlight.gameObject.SetActive(true);
            yield return new WaitForSeconds(m_HighlightTime);
            Highlight.gameObject.SetActive(false);
        }
    }
}
