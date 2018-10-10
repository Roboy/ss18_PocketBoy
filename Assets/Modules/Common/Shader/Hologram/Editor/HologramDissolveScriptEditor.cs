using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pocketboy.Common
{
    [CustomEditor(typeof(HologramDissolveScript))]
    public class HologramDissolveScriptEditor : Editor
    {
        HologramDissolveScript m_Script;

        private void Awake()
        {
            m_Script = (HologramDissolveScript)target;

            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Save Material"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save Material", "Material", "mat", "Material Saved");
                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(m_Script.HologramMaterial, path);
                }
            }
        }
    }
}


