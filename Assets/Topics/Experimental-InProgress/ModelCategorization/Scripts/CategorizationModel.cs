﻿using System.Collections;
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

        private Transform m_RespawnPose;

        public void OnPointerDown(PointerEventData eventData)
        {
            ModelCategorizationManager.Instance.ShowObjectName(m_Name);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!m_IsOnPlatform && m_Platform != null)
            {
                ModelCategorizationManager.Instance.ShowObjectName(m_Explanation);
                bool isOnCorrentPlatform = m_Platform.CheckContent(m_ContentRelatedState);
                if (isOnCorrentPlatform)
                {
                    var draggableObject = GetComponent<DraggableObject>();
                    if (draggableObject != null)
                        draggableObject.enabled = false;

                    m_IsOnPlatform = true;
                }
                else
                {
                    Handheld.Vibrate();
                }
            }
        }

        public void Setup(GameObject modelPrefab, string name, string explanation, ContentRelated state, Transform respawnPose)
        {
            m_Name = name;
            m_Explanation = explanation;
            m_ContentRelatedState = state;
            var model = Instantiate(modelPrefab, transform);
            m_RespawnPose = respawnPose;
            ResetPose();
        }

        private void ResetPose()
        {
            transform.position = m_RespawnPose.position;
            transform.rotation = m_RespawnPose.rotation;
        }

        void OnTriggerEnter(Collider other)
        {
            var platform = other.GetComponent<CategorizationPlatform>();
            if (platform != null)
            {
                m_Platform = platform;
                return;
            }

            if (other.CompareTag("Deadzone"))
            {
                Handheld.Vibrate();
                ResetPose();
            }


        }

        void OnTriggerExit(Collider other)
        {
            m_Platform = null;
        }
    }
}
