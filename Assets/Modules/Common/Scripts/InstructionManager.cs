using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Pocketboy.Common
{
    /// <summary>
    /// InstructionManager shows the Instruction in each scene at commands Roboy to read the instruction if not muted.
    /// TODO:: Push sentences which are split in following pages to the next page if it fits one page.
    /// </summary>
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

        private List<Canvas> m_SceneCanvases = new List<Canvas>();

        private void Awake()
        {
            m_PageController = GetComponent<PageController>();
            DontDestroyOnLoad(gameObject.transform.root);

            HelpButton.onClick.AddListener(ToggleInstruction);

            SceneManager.sceneUnloaded += (scene) =>
            {
                HelpButton.gameObject.SetActive(false);
                m_SceneCanvases.Clear();
            };

            RoboyEvents.RoboyFinishedTalkingEvent += NextInstruction;
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

        private void ToggleInstruction()
        {
            if (!m_IsActive)
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                ShowInstruction();
            }
            else
            {
                AudioSourcesManager.Instance.PlaySound("ButtonClick");
                HideInstruction();
            }
        }

        public void ShowInstruction()
        {
            if (m_IsActive)
                return;

            m_IsActive = true;
            SceneLoader.Instance.HideUI();
            ToggleSceneCanvases(false);
            transform.position = RoboyManager.Instance.InstructionPosition;
            transform.rotation = RoboyManager.Instance.transform.rotation;
            transform.forward = -transform.forward;
            InstructionText.pageToDisplay = 1;            
            InstructionCanvas.gameObject.SetActive(true);
            ReadInstruction();
        }

        public void HideInstruction()
        {
            if (!m_IsActive)
                return;

            m_IsActive = false;
            ToggleSceneCanvases(true);
            SceneLoader.Instance.ShowUI();
            InstructionCanvas.gameObject.SetActive(false);
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

            yield return new WaitForEndOfFrame();

            for (int i = 0; i < InstructionText.textInfo.pageCount; i++)
            {
                var page = InstructionText.textInfo.pageInfo[i];
                var pageText = InstructionText.text.Substring(page.firstCharacterIndex, page.lastCharacterIndex - page.firstCharacterIndex + 1);
                m_TextsPerPage.Add(pageText);
            }

            InstructionCanvas.gameObject.SetActive(false);
            InstructionCanvas.transform.localScale = cachedScale;
        }

        public void NextInstruction()
        {
            if (m_IsActive && m_PageController.NextPage(InstructionText))
            {
                ReadInstruction();
            }
        }

        public void PreviousInstruction()
        {
            if (m_IsActive && m_PageController.PreviousPage(InstructionText))
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

        private void ToggleSceneCanvases(bool enabledState)
        {
            if (m_SceneCanvases.Count == 0)
            {
                FindSceneCanvases();
            }
            foreach (var canvas in m_SceneCanvases)
            {
                canvas.enabled = enabledState;
            }                     
        }

        private void FindSceneCanvases()
        {
            m_SceneCanvases.Clear();
            var sceneCanvases = GameObject.FindGameObjectsWithTag("SceneUI");
            foreach (var sceneCanvas in sceneCanvases)
            {
                var canvas = sceneCanvas.GetComponent<Canvas>();
                if (canvas)
                {
                    m_SceneCanvases.Add(canvas);
                }
            }
        }
    }
}


