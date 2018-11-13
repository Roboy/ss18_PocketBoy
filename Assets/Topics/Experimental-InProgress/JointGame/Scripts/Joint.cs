using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public abstract class Joint : MonoBehaviour
    {
        [SerializeField]
        protected Transform EffectorParent;

        [SerializeField]
        protected float MotionDuration = 0f;

        protected bool m_IsMoving;

        protected Vector3 m_IdlePosition;

        protected Quaternion m_IdleRotation;

        private Coroutine m_Motion;

        protected virtual void Awake()
        {
            m_IdlePosition = transform.position;
            m_IdleRotation = transform.rotation;
        }

        public void ApplyMotion(Transform effector)
        {
            effector.SetParent(EffectorParent);
            effector.transform.localPosition = Vector3.zero;
            effector.transform.localRotation = Quaternion.identity;

            transform.position = m_IdlePosition;
            transform.rotation = m_IdleRotation;
            m_Motion = StartCoroutine(MotionAnimation());
        }

        public void StopMotion()
        {
            if (m_Motion != null)
                StopCoroutine(m_Motion);

            m_IsMoving = false;
        }

        protected abstract IEnumerator MotionAnimation();
    }
}


