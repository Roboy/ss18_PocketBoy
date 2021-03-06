﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Runaround
{

    public class PlayerStandingOnMe : MonoBehaviour
    {

        private RunaroundAnswer m_ans;

        private void Start()
        {
            if (GetComponent<RunaroundAnswer>() != null)
            {
                m_ans = GetComponent<RunaroundAnswer>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (other.tag == "Player")
            {
                GameMaster.Instance.CheckPosition(m_ans);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                GameMaster.Instance.CheckPosition(m_ans);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log(other.name);
            if (other.tag == "Player")
            {
                GameMaster.Instance.ResetPlane(m_ans);
            }
        }
    }
}
