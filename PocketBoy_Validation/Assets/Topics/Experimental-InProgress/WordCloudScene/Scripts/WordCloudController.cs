using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.Wordcloud
{

    public class WordCloudController : MonoBehaviour
    {

        [SerializeField]
        private WordCloud m_WC;
        private bool m_CloudSpawned = false;




        private void Initialize()
        {
            LevelManager.Instance.RegisterGameObjectWithRoboy(m_WC.gameObject, new Vector3(1.0f, 0.0f, -1.5f));
        }


        private void Update()
        {
            if (!m_CloudSpawned)
            {
                Initialize();
                m_CloudSpawned = true;
            }
        }

    }
}
