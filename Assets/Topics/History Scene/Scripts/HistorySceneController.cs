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
            if (TextToSpeechManager.Instance.IsTalking)
                return;

            TV.ShowContent(m_CurrentContentIndex);
            Slider.ShowDate(m_CurrentContentIndex);
            TextToSpeechManager.Instance.Talk(TextForSpeech[m_CurrentContentIndex]);
            m_CurrentContentIndex = MathUtility.WrapArrayIndex(m_CurrentContentIndex + 1, TextForSpeech.Count);
        }

        private void PreviousContent()
        {
            if (TextToSpeechManager.Instance.IsTalking)
                return;

            TV.ShowContent(MathUtility.WrapArrayIndex(m_CurrentContentIndex - 1, TextForSpeech.Count));
            Slider.ShowDate(MathUtility.WrapArrayIndex(m_CurrentContentIndex - 1, TextForSpeech.Count));
            TextToSpeechManager.Instance.Talk(TextForSpeech[MathUtility.WrapArrayIndex(m_CurrentContentIndex - 1, TextForSpeech.Count)]);
        }

        private void RepeatContent()
        {
            if (TextToSpeechManager.Instance.IsTalking)
                return;

            TV.RepeatContent();
            TextToSpeechManager.Instance.Talk(TextForSpeech[m_CurrentContentIndex]);
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
        }
    }
}

