using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;
using System;

namespace Pocketboy.Glassbreaker
{

    public class ScreenHealthController : Singleton<ScreenHealthController>
    {

        public Image CrackSprite;
        public Sprite[] CrackImages;

        public GameObject Screen;
        public GameObject DBMeasurement;
        public GameObject CalibrationInstructions;

        public Color Button_default;

        [SerializeField]
        private Button m_StartCalibrationButton;
        [SerializeField]
        private Button m_ResetButton;
        [SerializeField]
        private Button m_CalibrateButton;

        public TMPro.TextMeshProUGUI debugText;


        // Use this for initialization
        void Start()
        {
            m_StartCalibrationButton.onClick.AddListener(HandleStartCalibration);
            m_ResetButton.onClick.AddListener(HandleReset);
            m_CalibrateButton.onClick.AddListener(HandleCalibration);
            GameObject cam = GameObject.FindWithTag("MainCamera");
            Screen.transform.SetParent(cam.transform);
            Screen.transform.rotation = cam.transform.rotation;
            Screen.transform.position = cam.transform.position;
            

            Screen.transform.position += cam.transform.forward.normalized * 0.15f;
            LevelManager.Instance.RegisterObjectWithLevel(Screen);


        }

        private void HandleStartCalibration()
        {
            MeasureDB dbHandler = DBMeasurement.GetComponent<MeasureDB>();
            ToggleButtons("OFF");
            Vibration.Vibrate(100);
            ToggleCalibrationInstructions("ON");
            dbHandler.ResetTier();
            dbHandler.ResetCalibrationStatus();

        }

        private void HandleCalibration()
        {
            StartCoroutine(DBMeasurement.GetComponent<MeasureDB>().CalibrateMicrophone(2.0f));
        }


        private void HandleReset()
        {
            Vibration.Vibrate(100);
            Screen.SetActive(true);
            DBMeasurement.GetComponent<MeasureDB>().ResetTier();
        }


        public void SetScreenCrackTier(int tier)
        {
            if (tier < 0)
            {
                CrackSprite.sprite = CrackImages[0];
                return;
            }

            if (tier > 6)
            {
                CrackSprite.sprite = CrackImages[0];
                Screen.SetActive(false);
                Screen.GetComponentInChildren<Break>().BreakScreen();
                return;
            }

            CrackSprite.sprite = CrackImages[tier];

        }

        public void ToggleButtons(string operation)
        {

            if (operation == "ON")
            {
                m_StartCalibrationButton.enabled = true;
                m_StartCalibrationButton.GetComponent<Image>().color = Button_default;
                m_ResetButton.enabled = true;
                m_ResetButton.GetComponent<Image>().color = Button_default;
            }
            if (operation == "OFF")
            {
                m_StartCalibrationButton.enabled = false;
                m_StartCalibrationButton.GetComponent<Image>().color = Color.grey;
                m_ResetButton.enabled = false;
                m_ResetButton.GetComponent<Image>().color = Color.grey;
            }
        }

        public void ToggleCalibrationInstructions(string operation)
        {

            if (operation == "ON")
            {
                CalibrationInstructions.SetActive(true);
            }
            if (operation == "OFF")
            {
                CalibrationInstructions.SetActive(false);
            }

        }

        public void FillSlider(float amount)
        {
            CalibrationInstructions.GetComponentInChildren<Slider>().value = amount;
        }

        public void FadeRoboySprite(float amount)
        {
            Color tmp = m_CalibrateButton.GetComponent<Image>().color;
            tmp.a = amount;
            m_CalibrateButton.GetComponent<Image>().color = tmp;


        }

        public void ResetCalibrationInstructions()
        {
            CalibrationInstructions.GetComponentInChildren<Slider>().value = 0.0f;
            Color tmp = m_CalibrateButton.GetComponent<Image>().color;
            tmp.a = 1.0f;
            m_CalibrateButton.GetComponent<Image>().color = tmp;
        }


        public void PrintDebugText(string text)
        {
            debugText.text = text;
        }
    }
}
