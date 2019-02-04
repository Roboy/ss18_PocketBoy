using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class RotationalButton : MonoBehaviour
    {
        [SerializeField]
        private RectTransform MovingLink;

        [SerializeField]
        private float MaxRotation = 30f;

        [SerializeField]
        private float Speed = 50f;

        private Vector3 m_EulerAngles = Vector3.zero;

        void Update()
        {
            m_EulerAngles.z = Mathf.PingPong(Time.time * Speed, 2 * MaxRotation) - MaxRotation;
            MovingLink.eulerAngles = m_EulerAngles;
        }
    }
}
