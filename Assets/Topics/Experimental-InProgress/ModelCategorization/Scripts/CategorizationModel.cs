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
        private ContentRelated m_ContentRelatedState;

        private CategorizationPlatform m_Platform;

        private bool m_IsOnPlatform = false;

        private string m_Name = null;

        private string m_Explanation = null;

        public void OnPointerDown(PointerEventData eventData)
        {
            ModelCategorizationManager.Instance.ShowObjectInformation(m_Name, m_Explanation);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
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

        public void Setup(GameObject modelPrefab, string name, string explanation, ContentRelated state)
        {
            m_Name = name;
            m_Explanation = explanation;
            m_ContentRelatedState = state;
            var model = Instantiate(modelPrefab, transform);
        }

        void OnTriggerEnter(Collider other)
        {
            var platform = other.GetComponent<CategorizationPlatform>();
            if (platform != null)
            {
                m_Platform = platform;
            }
        }

        void OnTriggerExit(Collider other)
        {
            m_Platform = null;
        }
    }
}
