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
        private GameObject m_PlayerCollider;

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
            AudioSourcesManager.Instance.PlaySound("ButtonClick");
            GameMaster.Instance.StartRunaround(QuestionManager.Instance.GetCurrentQuestion().CorrectAnswerID);
        }

        private void Initialize()
        {
            //Attach playing field respectively to Roboy
            LevelManager.Instance.RegisterGameObjectWithRoboy(m_GM.gameObject, new Vector3(-2f, -0.5f, 1f));
            m_GM.transform.forward = RoboyManager.Instance.transform.forward * (-1f);


            //Attach collider to camera so that planes can be stepped on
            GameObject cam = GameObject.FindWithTag("MainCamera");
            m_PlayerCollider.transform.position = cam.transform.position;
            m_PlayerCollider.transform.SetParent(cam.transform);
            LevelManager.Instance.RegisterObjectWithLevel(m_PlayerCollider);


            //Init game logic
            GameMaster.Instance.SetInitProperties();
            QuestionManager.Instance.LoadQuestion(0);
            m_play.onClick.AddListener(Listen);

        }

        

    }
}
