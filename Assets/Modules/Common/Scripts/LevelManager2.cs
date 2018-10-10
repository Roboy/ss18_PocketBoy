using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

namespace Pocketboy.Common
{
    public class LevelManager2 : Singleton<LevelManager2>
    {
        [SerializeField]
        private RoboyController RoboyPrefab;

        private bool m_IsQuitting = false;

        private bool m_RoboySpawned = false;

        private DetectedPlane m_RoboyPlane;

        private RoboyController m_Roboy;


        void Update()
        {
            if(!m_RoboySpawned)
                CreateLevel();
        }

        private void CreateLevel()
        {
            //m_Roboy = Instantiate(RoboyPrefab, plane.CreateAnchor(plane.CenterPose).transform);
            //m_Roboy.transform.localPosition = Vector3.zero;
            //m_Roboy.transform.LookAt(Camera.main.transform.forward);
            m_RoboySpawned = true;
        }

      
    }
}


