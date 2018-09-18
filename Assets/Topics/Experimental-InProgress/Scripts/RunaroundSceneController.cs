using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;

namespace Pocketboy.Runaround
{

    public class RunaroundSceneController : MonoBehaviour
    {

        [SerializeField]
        private GameMaster m_GM;

        [SerializeField]
        private Button m_play;

        private void Awake()
        {
            Initialize();
        }

        public void Listen()
        {
            Debug.Log("Button Clicked.");
            GameMaster.Instance.StartRunaround(Random.Range(0, 3));
        }

        private void Initialize()
        {
            var roboy = LevelManager.Instance.Roboy;
            m_GM.transform.position = roboy.transform.position - roboy.transform.right * 6.65f;
            m_GM.transform.position = roboy.transform.position - 0.5f * roboy.transform.up;
            m_GM.transform.forward = roboy.transform.forward * (-1f);
            m_GM.transform.parent = roboy.transform.parent;
            Debug.Log(m_GM.transform.parent);
            m_play.onClick.AddListener(Listen);
        }

        

    }
}
