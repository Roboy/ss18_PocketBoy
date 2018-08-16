using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;

namespace Pocketboy.HistoryScene
{
    public class TimeSlider : MonoBehaviour
    {
        public int MaxDates { get { return MaxDatesVisible; } }

        public float PartSize { get { return SliderPartSize; } }


        [SerializeField, Tooltip("Part representing a visual date")]
        private TimeSliderPart SliderPart;

        [SerializeField]
        private TimeSliderEnd LeftEnd;

        [SerializeField]
        private TimeSliderEnd RightEnd;

        [SerializeField, Tooltip("Width of each part")]
        private int SliderPartSize;

        [SerializeField]
        private Color SliderColor = Color.white;

        [SerializeField, Tooltip("Mask container used in another script")]
        private RectTransform SliderContainer;

        [SerializeField]
        private int[] Dates;

        [SerializeField]
        private int MaxDatesVisible;

        [SerializeField, Range(0f, 1f)]
        private float DateSwitchTime = 0.5f;

        /// <summary>
        /// Cached list of all slider parts.
        /// </summary>
        private List<TimeSliderPart> m_SliderParts = new List<TimeSliderPart>();

        private TimeSliderEnd m_LeftEnd;

        private TimeSliderEnd m_RightEnd;

        /// <summary>
        /// Index of current shown date.
        /// </summary>
        private int m_CurrentDate = 0;

        private bool m_Changing;

        public void FillSlider(int[] dates)
        {
            // e.g. partsize = 200 with 5 parts => (200 * (5-1)) = 800 => 800 / 2 => 400 => first position = -400
            float x_position = 0f;
            Vector3 position = new Vector3(x_position, 0f, 0f);
            Vector2 size = new Vector3(SliderPartSize, SliderPartSize);
            SliderContainer.sizeDelta = new Vector2(SliderPartSize * MaxDatesVisible, SliderPartSize);
            for (int i = 0; i < dates.Length; i++)
            {
                var sliderPart = Instantiate(SliderPart, SliderContainer);
                sliderPart.Setup(this, SliderContainer, position, size, dates[i], SliderColor);
                m_SliderParts.Add(sliderPart);
                position.x += SliderPartSize; // move to next position
            }
            // create the ends
            m_RightEnd = Instantiate(RightEnd, SliderContainer);
            position.x = SliderPartSize * dates.Length;
            m_RightEnd.Setup(SliderContainer, position, size, SliderColor);

            m_LeftEnd = Instantiate(LeftEnd, SliderContainer);
            position.x = -SliderPartSize;
            m_LeftEnd.Setup(SliderContainer, position, size, SliderColor);
        }

        public void ShowNextDate()
        {
            ShowDate(m_CurrentDate + 1);
        }

        public void ShowPreviousDate()
        {
            ShowDate(m_CurrentDate - 1);
        }

        public void ShowDate(int index)
        {
            if (m_SliderParts.Count == 0)
                return;

            m_CurrentDate = MathUtility.WrapArrayIndex(index, m_SliderParts.Count);
            StartCoroutine(UpdateSlider());
        }

        private IEnumerator UpdateSlider()
        {
            while (m_Changing)
                yield return null;

            m_Changing = true;
            float currentTime = 0f;
            float positionChange = -m_SliderParts[m_CurrentDate].transform.localPosition.x; // we move the whole slider to the left so we need the opposite vector motion
            var posByPartIndex = new Dictionary<int, float>();
            float leftEndInitPos = m_LeftEnd.transform.localPosition.x;
            float rightEndInitPos = m_RightEnd.transform.localPosition.x;
            for (int i = 0; i < m_SliderParts.Count; i++)
            {
                posByPartIndex.Add(i, m_SliderParts[i].transform.localPosition.x);
            }
            float newPos = 0f;
            float factor = 0f;
            while (currentTime < DateSwitchTime)
            {
                factor = currentTime / DateSwitchTime;
                for (int i = 0; i < m_SliderParts.Count; i++)
                {
                    newPos = Mathf.SmoothStep(posByPartIndex[i], posByPartIndex[i] + positionChange, factor);
                    m_SliderParts[i].transform.localPosition = Vector3.right * newPos;
                }
                newPos = Mathf.SmoothStep(leftEndInitPos, leftEndInitPos + positionChange, factor);
                m_LeftEnd.transform.localPosition = Vector3.right * newPos;
                newPos = Mathf.SmoothStep(rightEndInitPos, rightEndInitPos + positionChange, factor);
                m_RightEnd.transform.localPosition = Vector3.right * newPos;
                currentTime += Time.deltaTime;
                yield return null;
            }
            for (int i = 0; i < m_SliderParts.Count; i++)
            {
                m_SliderParts[i].transform.localPosition = Vector3.right * (posByPartIndex[i] + positionChange);
            }
            m_LeftEnd.transform.localPosition = Vector3.right * (leftEndInitPos + positionChange);
            m_RightEnd.transform.localPosition = Vector3.right * (rightEndInitPos + positionChange);
            m_Changing = false;
        }
    }

}