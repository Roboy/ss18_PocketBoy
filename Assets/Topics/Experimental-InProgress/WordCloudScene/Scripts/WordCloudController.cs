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
<<<<<<< HEAD
            LevelManager.Instance.RegisterGameObjectWithRoboy(m_WC.gameObject);
            m_WC.transform.position = LevelManager.Instance.GetPositionRelativeToRoboy(new Vector3(1.0f, 0.0f, -2.5f));
=======
            LevelManager.Instance.RegisterGameObjectWithRoboy(m_WC.gameObject, new Vector3(-1f, 0.5f, -1.5f));

            //var roboy = LevelManager.Instance.m_Roboy;
            //m_WC.transform.position = roboy.transform.position;
            ////Move the cloud to the side of roboy
            //m_WC.transform.position -= 1.0f * roboy.transform.right;
            ////Move the cloud behind roboy
            //m_WC.transform.position -= 1.5f * roboy.transform.forward;
            ////Move the cloud above roboy
            //m_WC.transform.position += 0.5f * roboy.transform.up;
            ////m_WC.transform.forward = roboy.transform.forward * (-1f);
            //m_WC.transform.parent = roboy.transform.parent;
>>>>>>> 8b85f2a7c81e5a38c413d52490f84a9ce30b3b28
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
