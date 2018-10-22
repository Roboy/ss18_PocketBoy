using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHealthController : MonoBehaviour {

    public Image CrackSprite;
    public Sprite[] CrackImages;
    public Slider ValueSlider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateCracks()
    {
        CrackSprite.sprite = CrackImages[(int)ValueSlider.value];

    }
}
