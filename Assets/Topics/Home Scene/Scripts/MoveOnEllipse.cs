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

        private enum State
        {
            Stopped,
            Moving,
            Paused
        }

        private State m_MovementState = State.Stopped;

        private void Awake()
        {
            Setup();

            if (OnAwake)
                StartMoving();
        }

        private void OnDrawGizmosSelected()
        {
            EllipseObject.OnDrawGizmosSelected();
        }

        public void StartMoving()
        {
            if (EllipseObject == null)
                return;

            StartCoroutine(Move());
        }

        public void ResumeMoving()
        {
            if (m_MovementState != State.Paused)
                return;

            m_MovementState = State.Moving;
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

        private IEnumerator Move()
        {
            m_MovementState = State.Moving;
            int currentPointIndex = 0;
            int nextPointIndex = 1;

            float nextDistance = (m_Path[currentPointIndex] - m_Path[nextPointIndex]).magnitude;
            float timeForNextStep = nextDistance / m_AverageVelocity;
            
            float currentTime = 0f;
            Vector3 currentPosition;
     

            while (m_MovementState != State.Stopped)
            {
                if (m_MovementState == State.Paused)
                {
                    yield return null;
                }

                else
                {
                    if (currentTime >= timeForNextStep) // reached next point
                    {
                        currentPointIndex = nextPointIndex;
                        nextPointIndex = MathUtility.WrapArrayIndex(nextPointIndex + 1, m_Path.Length);

                        nextDistance = (m_Path[currentPointIndex] - m_Path[nextPointIndex]).magnitude;
                        timeForNextStep = nextDistance / m_AverageVelocity;

                        currentTime = 0f;
                    }

                    currentTime += Time.deltaTime;
                    currentPosition = Vector3.Lerp(m_Path[currentPointIndex], m_Path[nextPointIndex], currentTime / timeForNextStep); // lerp between currentPoint and next point of the ellipse path
                    if (!IsLocal)
                        transform.position = currentPosition;
                    else
                        transform.localPosition = currentPosition;
                    yield return null;
                }
            }
        }
    }
}