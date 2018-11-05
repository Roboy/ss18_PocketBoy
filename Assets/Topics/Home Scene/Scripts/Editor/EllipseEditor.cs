using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pocketboy.Common
{
    [CustomEditor(typeof(Ellipse))]
    public class EllipseEditor : Editor
    {
        private Ellipse m_Ellipse;

        private void Awake()
        {
            m_Ellipse = (Ellipse)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Save Ellipse"))
            {
                m_Ellipse.SaveEllipse();
            }
        }
    }
}
