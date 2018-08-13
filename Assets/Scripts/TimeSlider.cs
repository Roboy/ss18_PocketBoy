using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TimeSlider : MonoBehaviour {

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
    private List<Transform> m_SliderParts = new List<Transform>();

    private TimeSliderEnd m_LeftEnd;

    private TimeSliderEnd m_RightEnd;

    /// <summary>
    /// Index of current shown date.
    /// </summary>
    private int m_CurrentDate = 0;

    private bool m_Changing;

	// Use this for initialization
	void Start () {
        CreateSlider();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow)) { ShowNextDate(); }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) { ShowPreviousDate(); }
    }

    private void CreateSlider()
    {
        // e.g. partsize = 200 with 5 parts => (200 * (5-1)) = 800 => 800 / 2 => 400 => first position = -400
        float x_position = 0f;
        Vector3 position = new Vector3(x_position, 0f, 0f);
        Vector2 size = new Vector3(SliderPartSize, SliderPartSize);
        SliderContainer.sizeDelta = new Vector2(SliderPartSize * MaxDatesVisible, SliderPartSize);
        for (int i = 0; i < Dates.Length; i++)
        {
            var sliderPart = Instantiate(SliderPart, SliderContainer);
            sliderPart.Setup(SliderContainer, position, size, Dates[i], SliderColor);           
            m_SliderParts.Add(sliderPart.transform);
            position.x += SliderPartSize; // move to next position
        }
        // create the ends
        m_RightEnd = Instantiate(RightEnd, SliderContainer);
        position.x = SliderPartSize;
        m_RightEnd.Setup(position, size, SliderColor);
        m_RightEnd.ImageComponent.enabled = false;

        m_LeftEnd = Instantiate(LeftEnd, SliderContainer);
        position.x = -SliderPartSize;
        m_LeftEnd.Setup(position, size, SliderColor);
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
        if (Dates.Length == 0)
            return;

        m_CurrentDate = MathUtility.WrapArrayIndex(index, Dates.Length);
        StartCoroutine(UpdateSlider());
    }

    private IEnumerator UpdateSlider()
    {
        while (m_Changing)
            yield return null;

        m_Changing = true;
        if (m_CurrentDate == 0)
        {
            m_LeftEnd.Show(DateSwitchTime);            
        }
        else if (m_CurrentDate == m_SliderParts.Count - 1)
        {
            m_RightEnd.Show(DateSwitchTime);
            
        }
        else
        {
            m_LeftEnd.Hide(DateSwitchTime);
            m_RightEnd.Hide(DateSwitchTime);
        }
            
        float currentTime = 0f;
        float positionChange = -m_SliderParts[m_CurrentDate].localPosition.x; // we move the whole slider to the left so we need the opposite vector motion
        var posByPartIndex = new Dictionary<int, float>();
        for(int i = 0; i < m_SliderParts.Count; i++)
        {
            posByPartIndex.Add(i, m_SliderParts[i].localPosition.x);
        }
        while (currentTime < DateSwitchTime)
        {
            for (int i = 0; i < m_SliderParts.Count; i++)
            {
                var newPos = Mathf.SmoothStep(posByPartIndex[i], posByPartIndex[i] + positionChange, currentTime / DateSwitchTime);
                m_SliderParts[i].transform.localPosition = Vector3.right * newPos;
            }
            currentTime += Time.deltaTime;            
            yield return null;
        }
        for (int i = 0; i < m_SliderParts.Count; i++)
        {
            m_SliderParts[i].transform.localPosition = Vector3.right * (posByPartIndex[i] + positionChange);
        }
        m_Changing = false;
    }
}
