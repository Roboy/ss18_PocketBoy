using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;

namespace Pocketboy.HistoryScene
{ 
    /// <summary>
    /// Represents the left or the right end of the slider which has a faded out sprite.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class TimeSliderEnd : MonoBehaviour
    {
        [SerializeField]
        private SoftMaskScript SoftMask;

        public void Setup(RectTransform container, Vector2 position, Vector2 size, Color color)
        {
            SoftMask.MaskScalingRect = container;
            transform.localPosition = position;
            GetComponent<RectTransform>().sizeDelta = size;
            GetComponent<Image>().color = color;
        }
    }
}