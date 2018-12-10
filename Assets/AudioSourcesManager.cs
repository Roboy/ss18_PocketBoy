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

        private void Awake()
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }

        public void PlaySound(string identifier)
        {
            m_SM_UI.PlaySound(identifier);
            m_SM_3D_World.PlaySound(identifier);
        }
       
    }
}