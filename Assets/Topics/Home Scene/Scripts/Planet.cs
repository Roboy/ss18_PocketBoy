using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class Planet : MonoBehaviour
    {
        private MoveOnEllipse m_MovementController;

        private ScaleController m_ScaleController;

        private void Awake()
        {
            m_MovementController = GetComponent<MoveOnEllipse>();
            m_ScaleController = GetComponent<ScaleController>();
        }

        public void ZoomIn()
        {
            if(m_MovementController != null)
                m_MovementController.PauseMoving();

            if(m_ScaleController != null)
                m_ScaleController.Scale();
        }

        public void ZoomOut()
        {
            if (m_MovementController != null)
                m_MovementController.ResumeMoving();

            if (m_ScaleController != null)
                m_ScaleController.ResetScale();
        }
    }
}


