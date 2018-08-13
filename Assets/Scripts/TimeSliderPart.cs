using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TimeSliderPart : MonoBehaviour {

    [SerializeField]
    private SoftMaskScript SoftMask;

    [SerializeField]
    private Text Date;

    public void Setup(RectTransform container, Vector2 position, Vector2 size, int date, Color color)
    {
        SoftMask.MaskScalingRect = container;
        transform.localPosition = position;
        GetComponent<RectTransform>().sizeDelta = size;
        Date.text = date.ToString();
        Date.transform.localPosition = new Vector2(0f, size.y / 2f); // position above image
        Date.GetComponent<RectTransform>().sizeDelta = size;
        GetComponent<Image>().color = color;
    }
}
