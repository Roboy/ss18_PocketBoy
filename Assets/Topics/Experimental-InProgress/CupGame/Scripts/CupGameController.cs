using System.Collections;
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


        void Start()
        {



        }

        private void Update()
        {
            if (!m_ShufflerPlaced)
            {
                m_Shuffler.transform.SetParent(LevelManager.Instance.GetAnchorTransform());
                m_Shuffler.transform.position = LevelManager.Instance.GetPositionRelativeToRoboy(new Vector3(0.6f, 0f, 0f));             

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
                m_ShufflerPlaced = true;
            }
        }


    }
}
