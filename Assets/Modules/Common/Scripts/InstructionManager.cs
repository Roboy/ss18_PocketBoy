using System.Collections;
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
            SetupTextsPerPages();
            HelpButton.gameObject.SetActive(true);
        }

        public void ShowInstruction()
        {
            if (m_IsActive)
                return;

            m_IsActive = true;
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

        private void SetupTextsPerPages()
        {
            m_TextsPerPage.Clear();
            foreach (var page in InstructionText.textInfo.pageInfo)
            {
                Debug.Log(page.firstCharacterIndex + " : " + page.lastCharacterIndex);
                m_TextsPerPage.Add(InstructionText.text.Substring(page.firstCharacterIndex, page.lastCharacterIndex - page.firstCharacterIndex + 1));
            }

            foreach (var a in m_TextsPerPage)
                Debug.Log(a);
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
            RoboyManager.Instance.Talk(m_TextsPerPage[InstructionText.pageToDisplay]);
        }
    }
}


