using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class ForceScreenOrientation : Singleton<ForceScreenOrientation>
    {
        [SerializeField]
        private ScreenOrientation Orientation;

        [SerializeField]
        private bool OnAwake;

        private ScreenOrientation m_OriginalOrientation;
        // Use this for initialization
        void Awake()
        {
            m_OriginalOrientation = Screen.orientation;

            if (!OnAwake)
                return;
            
            Screen.orientation = Orientation;
        }

        void OnDisable()
        {
            ResetScreenOrientation();
        }
        
        public void SetScreenOrientation(ScreenOrientation orientation)
        {
            Screen.orientation = orientation;
        }

        public void ResetScreenOrientation()
        {
            Screen.orientation = m_OriginalOrientation;
        }
    }
}

