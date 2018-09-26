using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.Word
{

    public class WordCloudController : MonoBehaviour
    {

        [SerializeField]
        private WordCloud m_WC;


        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var roboy = LevelManager.Instance.Roboy;
            m_WC.transform.position = roboy.transform.position;
            //Move the cloud to the side of roboy
            m_WC.transform.position -= 1.0f * roboy.transform.right;
            //Move the cloud behind roboy
            m_WC.transform.position -= 1.5f * roboy.transform.forward;
            //Move the cloud above roboy
            m_WC.transform.position += 0.5f * roboy.transform.up;
            //m_WC.transform.forward = roboy.transform.forward * (-1f);
            m_WC.transform.parent = roboy.transform.parent;
            

        }

    }
}
