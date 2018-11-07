using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pocketboy.Common
{
    public class Ellipse : MonoBehaviour
    {
        [SerializeField]
        private float SemiMajor = 0.2f;

        [SerializeField]
        private float SemiMinor = 0.1f;

        [SerializeField]
        private int Resolution = 100;

        [SerializeField]
        private bool IsLocal = false;

        public Vector3[] WorldPath
        {
            get { return GetWorldPath(SemiMajor, SemiMinor, Resolution); }
        }

        public Vector3[] LocalPath
        {
            get { return GetLocalPath(SemiMajor, SemiMinor, Resolution); }
        }

        public Vector3[] SavedPath
        {
            get
            {
                if (!m_SavedEllipse.Equals(default(EllipseObject)))
                    return m_SavedEllipse.Path;
                else
                    return null;
            }
        }

        public float CurrentPathLength { get { return MathUtility.GetApproximateEllipseCircumference(SemiMajor, SemiMinor); } }

        public float SavedPathLengthApproximate
        {
            get
            {
                if (!m_SavedEllipse.Equals(default(EllipseObject)))
                    return MathUtility.GetApproximateEllipseCircumference(m_SavedEllipse.SemiMajor, m_SavedEllipse.SemiMinor);
                else
                    return 0f;
            }
        }

        public float SavedPathLength
        {
            get
            {
                if (!m_SavedEllipse.Equals(default(EllipseObject)))
                    return MathUtility.GetPathLength(m_SavedEllipse.Path);
                else
                    return 0f;
            }
        }

        [SerializeField, HideInInspector]
        private Vector3[] m_CurrentPath;

        [SerializeField, HideInInspector]
        private EllipseObject m_SavedEllipse;

        [SerializeField, HideInInspector]
        private Vector3[] m_SavedPathInspector;

        [Serializable]
        private struct EllipseObject
        {
            public EllipseObject(float semiMajor, float semiMinor, int resolution, Vector3[] path)
            {
                SemiMajor = semiMajor;
                SemiMinor = semiMinor;
                Resolution = resolution;
                Path = path;
            }

            public float SemiMajor;
            public float SemiMinor;
            public int Resolution;
            public Vector3[] Path;
        }

        public void SaveEllipse()
        {
            if (!IsValid(SemiMajor, SemiMinor, Resolution))
            {
                Debug.Log("Ellipse is not valid, cannot save!");
                return;
            }

            Vector3[] path = new Vector3[Resolution];
            if (IsLocal)
                path = GetLocalPath(SemiMajor, SemiMinor, Resolution);
            else
                path = GetWorldPath(SemiMajor, SemiMinor, Resolution);

            m_SavedEllipse = new EllipseObject(SemiMajor, SemiMinor, Resolution, path);
            m_SavedPathInspector = GetWorldPath(SemiMajor, SemiMinor, Resolution);

            //OnDrawGizmosSelected();
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            if (!IsValid(SemiMajor, SemiMinor, Resolution))
                return;
            
            float pathSphereRadius = MathUtility.GetApproximateEllipseCircumference(SemiMajor, SemiMinor) / Resolution * 0.1f;

            m_CurrentPath = GetWorldPath(SemiMajor, SemiMinor, Resolution);

            for (int i = 0; i < m_CurrentPath.Length; i++)
            {
                Gizmos.DrawWireSphere(m_CurrentPath[i], pathSphereRadius);
            }

            Gizmos.color = Color.green;
            if (!m_SavedEllipse.Equals(default(EllipseObject)))
            {
                float radius = MathUtility.GetApproximateEllipseCircumference(m_SavedEllipse.SemiMajor, m_SavedEllipse.SemiMinor) / m_SavedEllipse.Resolution * 0.1f;
                for (int i = 0; i < m_SavedPathInspector.Length; i++)
                {
                    Gizmos.DrawWireSphere(m_SavedPathInspector[i], radius);
                }
            }
        }

        private bool IsValid(float semiMajor, float semiMinor, int resolution)
        {
            return (semiMajor > 0 && semiMinor > 0 && resolution > 3);
        }

        private Vector3[] GetPath(float semiMajor, float semiMinor, int resolution)
        {
            if (IsLocal)
                return GetLocalPath(semiMajor, semiMinor, resolution);
            else
                return GetWorldPath(semiMajor, semiMinor, resolution);
        }

        private Vector3[] GetWorldPath(float semiMajor, float semiMinor, int resolution)
        {
            Vector3[] positions = new Vector3[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float angle = i / (float)resolution * 2f * Mathf.PI;
                positions[i] = new Vector3(semiMajor * Mathf.Cos(angle), semiMinor * Mathf.Sin(angle), 0f);
                positions[i] = transform.rotation * positions[i] + transform.position;
            }
            return positions;
        }

        private Vector3[] GetLocalPath(float semiMajor, float semiMinor, int resolution)
        {
            Vector3[] positions = new Vector3[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float angle = i / (float)resolution * 2f * Mathf.PI;
                positions[i] = new Vector3(semiMajor * Mathf.Cos(angle), semiMinor * Mathf.Sin(angle), 0f);
            }
            return positions;
        }
    }
}


