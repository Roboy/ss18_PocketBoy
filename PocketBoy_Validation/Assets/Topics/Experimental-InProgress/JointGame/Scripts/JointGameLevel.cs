using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class JointGameLevel : MonoBehaviour
    {
        [SerializeField]
        private RoboticArmController RoboticArm;

        public int Points {
            get {
                if (m_Points != null)
                    return m_Points.Length;
                else
                    return -1;
            }
        }

        private JointGamePoint[] m_Points;

        private int m_HittedPointCount;

        private void Awake()
        {
            m_Points = GetComponentsInChildren<JointGamePoint>(true);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            foreach(var point in m_Points)
            {
                point.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            RoboticArm.StopMotion();
            gameObject.SetActive(false);
        }        
    }
}