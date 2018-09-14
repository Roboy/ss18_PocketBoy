using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class FaceController : MonoBehaviour
    {
        [SerializeField]
        private List<Sprite> Mouths = new List<Sprite>();

        [SerializeField]
        private float Speed = 0.1f;

        private SpriteRenderer m_Renderer;

        private bool m_MouthAnimation;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_Renderer = GetComponent<SpriteRenderer>();

        }

        public void StartTalkAnimation()
        {
            if (m_MouthAnimation)
                return;

            m_MouthAnimation = true;
            StartCoroutine(StartTalkAnimationInternal());
        }

        public void StopTalkAnimation()
        {
            m_MouthAnimation = false;
        }

        private IEnumerator StartTalkAnimationInternal()
        {
            while (m_MouthAnimation)
            {
                for (int i = 0; i < Mouths.Count; i++)
                {
                    m_Renderer.sprite = Mouths[i];
                    yield return new WaitForSeconds(Speed);
                }
            }
            m_Renderer.sprite = Mouths[0];
        }
    }
}
