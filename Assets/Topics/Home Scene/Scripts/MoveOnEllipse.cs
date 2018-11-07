using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class MoveOnEllipse : MonoBehaviour
    {
        [SerializeField]
        private Ellipse EllipseObject;

        [SerializeField]
        private bool OnAwake = false;

        [SerializeField]
        private bool IsLocal = false;

        [SerializeField]
        private float CycleDuration = 5f;

        private Vector3[] m_Path;

        private float m_AverageVelocity;

        private float m_StartPositionInPercentage = 0f;

        private int m_CurrentPointIndex;

        private int m_NextPointIndex;

        private float m_NextDistance = 0f;

        private float m_TimeForNextStep = 0f;

        private float m_CurrentTime;

        private Vector3 m_CurrentPosition;

        private enum State
        {
            Stopped,
            Moving,
            Paused
        }

        private State m_MovementState = State.Stopped;

        private void Awake()
        {
            if (OnAwake)
            {
                Setup();
                StartMoving();               
            }
        }

        private void Update()
        {
            if (m_MovementState != State.Moving)
                return;

            if (m_CurrentTime >= m_TimeForNextStep) // reached next point
            {
                m_CurrentPointIndex = m_NextPointIndex;
                m_NextPointIndex = MathUtility.WrapArrayIndex(m_NextPointIndex + 1, m_Path.Length);

                m_NextDistance = (m_Path[m_CurrentPointIndex] - m_Path[m_NextPointIndex]).magnitude;
                m_TimeForNextStep = m_NextDistance / m_AverageVelocity;

                m_CurrentTime = 0f;
            }

            m_CurrentTime += Time.deltaTime;
            m_CurrentPosition = Vector3.Lerp(m_Path[m_CurrentPointIndex], m_Path[m_NextPointIndex], m_CurrentTime / m_TimeForNextStep); // lerp between currentPoint and next point of the ellipse path
            if (!IsLocal)
                transform.position = m_CurrentPosition;
            else
                transform.localPosition = m_CurrentPosition;
                
            
        }

        private void OnDrawGizmosSelected()
        {
            EllipseObject.OnDrawGizmosSelected();
        }

        public void StartMoving()
        {
            if (EllipseObject == null)
                return;

            m_MovementState = State.Moving;

            m_CurrentPointIndex = MathUtility.WrapArrayIndex((int)((m_Path.Length - 1) * m_StartPositionInPercentage), m_Path.Length); // convert value from [0,1] to [0, m_Path.Length-1] for the starting point
            m_NextPointIndex = MathUtility.WrapArrayIndex(m_CurrentPointIndex + 1, m_Path.Length);

            m_NextDistance = (m_Path[m_CurrentPointIndex] - m_Path[m_NextPointIndex]).magnitude;
            m_TimeForNextStep = m_NextDistance / m_AverageVelocity;
        }

        public void ResumeMoving()
        {
            switch (m_MovementState)
            {
                case State.Stopped:
                    StartMoving();
                    break;
                case State.Paused:
                    m_MovementState = State.Moving;
                    break;
                case State.Moving:
                    break;
            }
        }

        public void PauseMoving()
        {
            if (m_MovementState != State.Moving)
                return;

            m_MovementState = State.Paused;
        }

        public void StopMoving()
        {
            m_MovementState = State.Stopped;
        }

        public void SetEllipse(Ellipse ellipseObject, float cycleDuration, float startPositionInPercentage, bool isLocal)
        {
            EllipseObject = ellipseObject;
            CycleDuration = cycleDuration;
            IsLocal = isLocal;
            m_StartPositionInPercentage = Mathf.Clamp01(startPositionInPercentage);
            Setup();           
        }

        private void Setup()
        {
            m_Path = EllipseObject.SavedPath; // get the saved path, if not saved yet, get the runtime path, can be either in world or local space
            if (m_Path == null)
            {
                if (IsLocal)
                    m_Path = EllipseObject.LocalPath;
                else
                    m_Path = EllipseObject.WorldPath;
            }

            m_AverageVelocity = EllipseObject.SavedPathLength / CycleDuration;
        }
        
    }
}