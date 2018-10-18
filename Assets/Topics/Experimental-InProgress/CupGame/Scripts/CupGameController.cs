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

            if (LevelManager.Instance.Roboy != null)
            {
                PositionCups();
            }

        }

        private void Update()
        {
            if (LevelManager.Instance.Roboy != null && !m_ShufflerPlaced)
            {
                m_Shuffler.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(0.6f, 0f, 0f));
                m_Shuffler.transform.SetParent(LevelManager.Instance.GetAnchorTransform(), true);
                DragMe[] dms = m_Shuffler.GetComponentsInChildren<DragMe>();
                foreach (DragMe d in dms)
                {
                    d.resetOriginalPosition();
                }
                m_ShufflerPlaced = true;
            }
        }


        public void PositionCups()
        {





        }
    }
}
