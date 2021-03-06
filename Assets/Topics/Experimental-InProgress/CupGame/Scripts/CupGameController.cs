﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.Cupgame
{
    public class CupGameController : MonoBehaviour
    {
        [SerializeField]
        GameObject m_Shuffler;
        bool m_ShufflerPlaced = false;


        private void Update()
        {
            if (!m_ShufflerPlaced)
            {
                LevelManager.Instance.RegisterGameObjectWithRoboy(m_Shuffler, Vector3.zero, Quaternion.identity);
                Vector3 direction = RoboyManager.Instance.transform.forward;
                m_Shuffler.transform.rotation = Quaternion.LookRotation(direction * -1.0f);
                //m_Shuffler.transform.position += m_Shuffler.transform.forward.normalized * (-1.0f);
                m_Shuffler.transform.position += m_Shuffler.transform.right * (-0.75f);


                //var roboy = LevelManager.Instance.Roboy;
                //m_Shuffler.transform.position = roboy.transform.position;
                ////Move the cloud to the side of roboy
                //m_Shuffler.transform.position -= 1.0f * roboy.transform.right;
                ////Move the cloud behind roboy
                //m_Shuffler.transform.position -= 1.5f * roboy.transform.forward;
                ////Move the cloud above roboy
                //m_Shuffler.transform.position += 0.5f * roboy.transform.up;


                //Reset the init positions of the cup
                DragMe[] dms = m_Shuffler.GetComponentsInChildren<DragMe>();
                foreach (DragMe d in dms)
                {
                    d.resetOriginalPosition();
                }
                Wager.Instance.resetOriginPosition();
                m_ShufflerPlaced = true;
            }
        }


    }
}
