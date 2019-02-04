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

        public bool IsMoving { get; protected set; }

        protected Vector3 m_IdlePosition;

        protected Quaternion m_IdleRotation;

        protected virtual void Awake()
        {
            m_IdlePosition = transform.localPosition;
            m_IdleRotation = transform.localRotation;
        }

        private void FixedUpdate()
        {
            if (!IsMoving)
                return;

            UpdateMotion();
        }

        public void ApplyMotion(Transform effector)
        {
            effector.SetParent(EffectorParent);
            effector.transform.localPosition = Vector3.zero;
            effector.transform.localRotation = Quaternion.identity;

            transform.localPosition = m_IdlePosition;
            transform.localRotation = m_IdleRotation;
            StartMotion();
        }



        public void StopMotion()
        {
            IsMoving = false;
            transform.localPosition = m_IdlePosition;
            transform.localRotation = m_IdleRotation;
        }

        protected abstract void StartMotion();

        protected abstract void UpdateMotion();
    }
}


