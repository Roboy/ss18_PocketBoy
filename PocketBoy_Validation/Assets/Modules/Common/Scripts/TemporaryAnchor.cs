using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

namespace Pocketboy.Common
{
    public class TemporaryAnchor : MonoBehaviour
    {
        public GoogleARCore.Anchor m_RealAnchor;

        private List<Transform> m_AnchorChildren = new List<Transform>();

        private SessionStatus m_PreviousStatus = SessionStatus.None;

        private float m_TrackingLookupTime = 2f;

        private float m_TimeSinceLastLookup = 0f;

        private void Update()
        {
            m_TimeSinceLastLookup += Time.unscaledDeltaTime;
            if (m_TimeSinceLastLookup > m_TrackingLookupTime)
            {
                m_TimeSinceLastLookup = 0f;
                CheckTrackingStatus();
            }
        }

        public static void Create(Anchor anchor, bool dontDestroyOnLoad)
        {
            var temporaryAnchor = new GameObject("Temporary Anchor");
            temporaryAnchor.AddComponent<TemporaryAnchor>().Setup(anchor, dontDestroyOnLoad);
        }

        private void Setup(Anchor realAnchor, bool dontDestroyOnLoad)
        {
            m_RealAnchor = realAnchor;
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(transform);
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void CheckTrackingStatus()
        {
            if(m_RealAnchor == null || m_RealAnchor.TrackingState != TrackingState.Tracking) // todo, check session tracking state and anchor tracking state, if session is tracking again but anchor is not => create new anchor
                CreateNewAnchor();
        }

        private void CacheAnchorChildren()
        {
            foreach (Transform child in m_RealAnchor.transform)
            {
                m_AnchorChildren.Add(child);
            }
        }

        private void AttachChildrenToRealAnchor()
        {
            foreach (Transform child in m_AnchorChildren)
            {
                child.SetParent(m_RealAnchor.transform);
            }
        }

        private void CreateNewAnchor()
        {
            Debug.Log("Creating new Anchor!");
            CacheAnchorChildren();
            var plane = ARSessionManager.Instance.FloorPlane;
            m_RealAnchor = plane.CreateAnchor(plane.CenterPose);
            AttachChildrenToRealAnchor();
        }
    }
}
