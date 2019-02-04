using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    [RequireComponent(typeof(MoveOnEllipse), typeof(ScaleController))]
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
            m_MovementController.PauseMoving();
            m_ScaleController.Scale();
        }

        public void ZoomOut()
        {
            m_MovementController.ResumeMoving();
            m_ScaleController.ResetScale();
        }

        public void Setup(Ellipse ellipse, float cycleDuration, float scaleFactor)
        {
            m_MovementController.SetEllipse(ellipse, cycleDuration, Random.Range(0f, 1f), true);
            m_ScaleController.SetScaleFactor(scaleFactor);
        }
    }
}


