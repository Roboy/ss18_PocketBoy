using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Pocketboy.Common
{

    public class SoundsManager : MonoBehaviour
    {

        [SerializeField]
        private AudioSource m_AudioChannel;

        public List<SoundClip> m_Sounds;

        private void Start()
        {

            if (GetComponent<AudioSource>() != null)
                m_AudioChannel = GetComponent<AudioSource>();
        }

        public void PlaySound(string ID)
        {
            m_AudioChannel.Stop();
            m_AudioChannel.clip = null;

            foreach (SoundClip sc in m_Sounds)
            {
                if (sc.ID == ID)
                    m_AudioChannel.clip = sc.AudioFile;
            }

            if (m_AudioChannel.clip != null)
                m_AudioChannel.Play();
        }
    }
}
