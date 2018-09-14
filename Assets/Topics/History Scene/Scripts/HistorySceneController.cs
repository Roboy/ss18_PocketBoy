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
            if (RoboyManager.Instance.IsTalking)
                return;

            if(m_IsOn)
                m_CurrentContentIndex = MathUtility.WrapArrayIndex(m_CurrentContentIndex + 1, TextForSpeech.Count);

            m_IsOn = true;
            ShowContent(m_CurrentContentIndex);
        }

        private void PreviousContent()
        {
            if (RoboyManager.Instance.IsTalking)
                return;

            m_CurrentContentIndex = MathUtility.WrapArrayIndex(m_CurrentContentIndex - 1, TextForSpeech.Count);
            ShowContent(m_CurrentContentIndex);           
        }

        private void RepeatContent()
        {
            if (RoboyManager.Instance.IsTalking)
                return;

            TV.RepeatContent();
            RoboyManager.Instance.Talk(TextForSpeech[m_CurrentContentIndex]);
        }

        private void ShowContent(int index)
        {
            TV.ShowContent(m_CurrentContentIndex);
            Slider.ShowDate(m_CurrentContentIndex);
            RoboyManager.Instance.Talk(TextForSpeech[m_CurrentContentIndex]);
            
        }

        private void Initialize()
        {
            List<Object> tvContent = new List<Object>();
            List<int> dates = new List<int>();
            foreach (var content in Content)
            {
                tvContent.Add(content.TVContent);
                dates.Add(content.Date);
                TextForSpeech.Add(content.Text);
            }
            TV.FillContent(tvContent.ToArray());
            Slider.FillSlider(dates.ToArray());

            Debug.Log("DICK 1: " + LevelManager.Instance.name);
            var roboy = LevelManager.Instance.Roboy;
            Debug.Log("DICK 2: " + LevelManager.Instance.Roboy.name);
            TV.transform.position = roboy.transform.position + roboy.transform.right * 0.65f;
            TV.transform.forward = roboy.transform.forward;

            RoboyManager.Instance.Talk("Thank you for tuning in. " +
                "This programm will tell you about some awesome milestones in the evolution of robots. " +
                "In order to change the milestones just click on the buttons attached to the television.");
        }
    }
}

