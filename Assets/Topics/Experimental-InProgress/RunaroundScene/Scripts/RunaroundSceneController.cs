﻿using System.Collections;
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
            GameMaster.Instance.StartRunaround(QuestionManager.Instance.GetCurrentQuestion().CorrectAnswerID);
        }

        private void Initialize()
        {
            LevelManager.Instance.RegisterGameObjectWithRoboy(m_GM.gameObject, new Vector3(-2f, -0.5f, 1f));
            m_GM.transform.forward = RoboyManager.Instance.transform.forward * (-1f);
            //m_GM.transform.position = roboy.transform.position - roboy.transform.right * 2.0f;
            //m_GM.transform.position += 1.0f * roboy.transform.forward;
            //m_GM.transform.position -= 0.5f * roboy.transform.up;
            //m_GM.transform.parent = roboy.transform.parent;


            GameMaster.Instance.SetInitProperties();
            QuestionManager.Instance.LoadQuestion(0);
            m_play.onClick.AddListener(Listen);

        }

        

    }
}
