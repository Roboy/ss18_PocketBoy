using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Pocketboy.Common
{

    public class AudioSourcesManager : Singleton<AudioSourcesManager>
    {
        [SerializeField]
        private SoundsManager m_SM_UI;

        [SerializeField]
        private SoundsManager m_SM_3D_World;

        private bool m_Initialized;

        private void Awake()
        {
            DontDestroyOnLoad(transform.root.gameObject);
            Initialize();
        }

        public void PlaySound(string identifier)
        {
            if (!m_Initialized)
                return;

            m_SM_UI.PlaySound(identifier);
            m_SM_3D_World.PlaySound(identifier);
        }

        private void Initialize()
        {
            m_Initialized = m_SM_UI != null && m_SM_3D_World != null;
        }
    }
}