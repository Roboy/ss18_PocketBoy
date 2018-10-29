using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pocketboy.Common;

namespace Pocketboy.Common
{
    [CustomEditor(typeof(VertexCount))]
    public class VertexCountEditor : Editor
    {
        VertexCount m_VertexCount;

        private void Awake()
        {
            m_VertexCount = (VertexCount)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Print Vertex Count"))
            {
                Debug.Log(m_VertexCount.GetVertexCount());
            }
        }
    }
}

