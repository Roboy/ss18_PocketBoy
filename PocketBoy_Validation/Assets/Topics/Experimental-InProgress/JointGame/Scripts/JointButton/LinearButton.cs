using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class LinearButton : MonoBehaviour
    {
        [SerializeField]
        private RectTransform MovingLink;

        [SerializeField]
        private float EndPosition;

        [SerializeField]
        private float Speed = 50f;

        private Vector3 m_LinkPosition = Vector3.zero;

        private void Update()
        {
            m_LinkPosition.y = -Mathf.PingPong(Time.time * Speed, EndPosition);
           // Debug.Log(Time.time);
            MovingLink.anchoredPosition3D = m_LinkPosition;
        }
    }
}
