using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Pocketboy.ModelCategorization
{
    public class CategorizationModel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private GameObject NameContainer;

        [SerializeField]
        private TextMeshProUGUI Name;

        [SerializeField]
        private ContentRelated m_ContentRelatedState;

        private CategorizationPlatform m_Platform;

        private bool m_IsOnPlatform = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            ShowName();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HideName();

            if (!m_IsOnPlatform && m_Platform != null)
            {
                bool isOnCorrentPlatform = m_Platform.CheckContent(m_ContentRelatedState);
                if (isOnCorrentPlatform)
                {
                    var draggableObject = GetComponent<DraggableObject>();
                    if (draggableObject != null)
                        draggableObject.enabled = false;

                    m_IsOnPlatform = true;
                }
            }
        }

        public void ShowName()
        {
            NameContainer.gameObject.SetActive(true);
        }

        public void HideName()
        {
            NameContainer.gameObject.SetActive(false);
        }

        public void SetName(string name)
        {
            Name.text = name;
        }

        public void OnTriggerEnter(Collider other)
        {
            var platform = other.GetComponent<CategorizationPlatform>();
            if (platform != null)
            {
                m_Platform = platform;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            m_Platform = null;
        }
    }
}
