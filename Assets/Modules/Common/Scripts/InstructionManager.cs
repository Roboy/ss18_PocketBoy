﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Pocketboy.Common
{
    [RequireComponent(typeof(PageController))]
    public class InstructionManager : Singleton<InstructionManager>
    {
        [SerializeField]
        private Canvas InstructionCanvas;

        [SerializeField]
        private TextMeshProUGUI InstructionText;

        [SerializeField]
        private Button HelpButton;

        private PageController m_PageController;

        private bool m_IsMuted;

        private bool m_IsActive;

        private List<string> m_TextsPerPage = new List<string>();

        private void Awake()
        {
            m_PageController = GetComponent<PageController>();
            DontDestroyOnLoad(gameObject.transform.root);

            SceneManager.sceneUnloaded += (scene) =>
            {
                HelpButton.gameObject.SetActive(false);
                HelpButton.interactable = true;
            };
        }

        public void PushInstruction(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            InstructionText.text = text;
            InstructionText.pageToDisplay = 1;
            StartCoroutine(SetupTextsPerPages());
            HelpButton.gameObject.SetActive(true);
        }

        public void ShowInstruction()
        {
            if (m_IsActive)
                return;

            m_IsActive = true;
            transform.position = RoboyManager.Instance.InstructionPosition;
            InstructionText.pageToDisplay = 1;            
            InstructionCanvas.gameObject.SetActive(true);
            HelpButton.interactable = false;
            ReadInstruction();
        }

        public void HideInstruction()
        {
            if (!m_IsActive)
                return;

            m_IsActive = false;
            InstructionCanvas.gameObject.SetActive(false);
            HelpButton.interactable = true;
            RoboyManager.Instance.StopTalking();
        }

        private IEnumerator SetupTextsPerPages()
        {
            m_TextsPerPage.Clear();

            // HACK: pageInfo.lastCharacterIndex will get updated as soon as text gets active in the scene
            // activate object with 0 scale so its not visible
            Vector3 cachedScale = InstructionCanvas.transform.localScale;
            InstructionCanvas.transform.localScale = Vector3.zero;
            InstructionCanvas.gameObject.SetActive(true);

            while (InstructionText.textInfo.pageInfo[0].lastCharacterIndex == 0)
                yield return null;

            for (int i = 0; i < InstructionText.textInfo.pageCount; i++)
            {
                var page = InstructionText.textInfo.pageInfo[i];
                m_TextsPerPage.Add(InstructionText.text.Substring(page.firstCharacterIndex, page.lastCharacterIndex - page.firstCharacterIndex + 1));
            }

            InstructionCanvas.gameObject.SetActive(false);
            InstructionCanvas.transform.localScale = cachedScale;
        }

        public void NextInstruction()
        {
            if (m_PageController.NextPage(InstructionText))
            {
                ReadInstruction();
            }
        }

        public void PreviousInstruction()
        {
            if (m_PageController.PreviousPage(InstructionText))
            {
                ReadInstruction();
            }
        }

        public void Mute()
        {
            m_IsMuted = true;
            RoboyManager.Instance.StopTalking();           
        }

        public void Unmute()
        {          
            m_IsMuted = false;
            ReadInstruction();
        }

        private void ReadInstruction()
        {
            if (m_IsMuted)
                return;

            RoboyManager.Instance.StopTalking();
            RoboyManager.Instance.Talk(m_TextsPerPage[InstructionText.pageToDisplay-1]); // TextMeshPro text pages go from [1, pageCount]
        }
    }
}


