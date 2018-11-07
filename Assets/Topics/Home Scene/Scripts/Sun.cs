using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    [RequireComponent(typeof(ScaleController))]
    public class Sun : MonoBehaviour
    {
        private ScaleController m_ScaleController;

        private void Awake()
        {
            m_ScaleController = GetComponent<ScaleController>();
        }

        public void ZoomIn()
        {
            m_ScaleController.Scale();
        }

        public void ZoomOut()
        {
            m_ScaleController.ResetScale();
        }
    }
}
