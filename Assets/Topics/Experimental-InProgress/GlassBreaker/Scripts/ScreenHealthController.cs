using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;
using System;

public class ScreenHealthController : Singleton<ScreenHealthController> {

    public Image CrackSprite;
    public Sprite[] CrackImages;

    public GameObject Screen;
    public GameObject DBMeasurement;

    [SerializeField]
    private Button m_CalibrateButton;
    [SerializeField]
    private Button m_ResetButton;

    public Text debugText;


    // Use this for initialization
    void Start () {
        m_CalibrateButton.onClick.AddListener(HandleCalibration);
        m_ResetButton.onClick.AddListener(HandleReset);

        GameObject cam = GameObject.FindWithTag("MainCamera");
        Screen.transform.rotation = cam.transform.rotation;
        Screen.transform.SetParent(cam.transform);

        Screen.transform.localPosition = cam.transform.forward.normalized * 0.15f;
        LevelManager.Instance.RegisterObjectWithLevel(Screen);


    }

    private void HandleCalibration()
    {
        Vibration.Vibrate(100);
        DBMeasurement.GetComponent<MeasureDB>().CalibrateMicrophone();
    }


    private void HandleReset()
    {
        Vibration.Vibrate(100);
        Screen.SetActive(true);
        //Screen.GetComponentInChildren<Break>().ResetScreen();
        DBMeasurement.GetComponent<MeasureDB>().ResetTier();
    }


    public void SetScreenCrackTier(int tier)
    {
        if (tier < 0)
        {
            CrackSprite.sprite = CrackImages[0];
            return;
        }

        if ( tier > 6)
        {
            CrackSprite.sprite = CrackImages[0];
            Screen.SetActive(false);
            Screen.GetComponentInChildren<Break>().BreakScreen();
            return;
        }

        CrackSprite.sprite = CrackImages[tier];

    }

}
