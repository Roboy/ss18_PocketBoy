using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using Pocketboy.Common;

namespace Pocketboy.HistoryScene
{
    public class HistorySceneController : MonoBehaviour
    {
        [SerializeField]
        private TVController TV;

        [SerializeField]
        private TimeSlider Slider;

        [SerializeField]
        private List<HistoryContent> Content = new List<HistoryContent>();

        [SerializeField]
        private List<string> TextForSpeech = new List<string>();

        private int m_CurrentContentIndex = 0;

        private bool m_IsOn = false;

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            EventManager.NextContentDelegate += NextContent;
            EventManager.PreviousContentDelegate += PreviousContent;
            EventManager.RepeatContentDelegate += RepeatContent;
        }

        private void OnDestroy()
        {
            EventManager.NextContentDelegate -= NextContent;
            EventManager.PreviousContentDelegate -= PreviousContent;
            EventManager.RepeatContentDelegate -= RepeatContent;
        }

        private void NextContent()
        {
            if(m_IsOn)
                m_CurrentContentIndex = MathUtility.WrapArrayIndex(m_CurrentContentIndex + 1, TextForSpeech.Count);

            m_IsOn = true;
            ShowContent(m_CurrentContentIndex);
        }

        private void PreviousContent()
        {
            m_CurrentContentIndex = MathUtility.WrapArrayIndex(m_CurrentContentIndex - 1, TextForSpeech.Count);
            ShowContent(m_CurrentContentIndex);           
        }

        private void RepeatContent()
        {
            AudioSourcesManager.Instance.PlaySound("ButtonClick");
            TV.RepeatContent();
            RoboyManager.Instance.Talk(TextForSpeech[m_CurrentContentIndex]);
        }

        private void ShowContent(int index)
        {
            AudioSourcesManager.Instance.PlaySound("TVChannelSwitch");
            TV.ShowContent(m_CurrentContentIndex);

            Slider.ShowDate(m_CurrentContentIndex);
            RoboyManager.Instance.Talk(TextForSpeech[m_CurrentContentIndex]);
            
        }

        private void Initialize()
        {
            List<Object> tvContent = new List<Object>();
            List<string> dates = new List<string>();
            foreach (var content in Content)
            {
                tvContent.Add(content.TVContent);
                dates.Add(content.Date);
                TextForSpeech.Add(content.Text);
            }
            TV.FillContent(tvContent.ToArray());
            Slider.FillSlider(dates.ToArray());

            LevelManager.Instance.RegisterGameObjectWithRoboy(TV.gameObject, new Vector3(0.65f, 0.25f, 0f));
            TV.transform.forward = RoboyManager.Instance.transform.forward;

            //German localization
            RoboyManager.Instance.Talk("Vielen Dank fürs Einschalten. " +
                "Ich bin Roboy und zusammen werden wir eine kleine Zeitreise durch die Entwicklung der Robotik machen. " +
                "Mit den Pfeiltasten am Fernseher kannst du vor und zurück, tippe einfach darauf, um den nächsten Beitrag zu starten.");

            ////English localization
            //RoboyManager.Instance.Talk("Thank you for tuning in. " +
            //   "This programm will tell you about some awesome milestones in the evolution of robots. " +
            //   "In order to change the milestones just click on the buttons attached to the television.");
        }
    }
}

