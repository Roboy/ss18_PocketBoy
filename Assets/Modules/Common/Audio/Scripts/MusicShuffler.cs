using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{

    public class MusicShuffler : MonoBehaviour
    {
        [SerializeField]
        private AudioSource m_Music = null;

        [SerializeField]
        private List<AudioClip> m_Songs;

        private void Start()
        {
            if(GetComponent<AudioSource>()!=null)
                m_Music = GetComponent<AudioSource>();

            if (m_Music != null)
            {
                PlayRandomSong();
            }
        }

        private void Update()
        {
            if (m_Music.isPlaying)
                return;

            if(!m_Music.isPlaying)
                PlayRandomSong();
        }

        private void PlayRandomSong()
        {
            m_Music.Stop();
            m_Music.clip = m_Songs[Random.Range(0, m_Songs.Count)];
            m_Music.Play();

        }

    }
}
