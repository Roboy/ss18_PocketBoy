using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pocketboy.Common
{
    public class Ellipse : MonoBehaviour
    {
        [SerializeField]
        private float SemiMajor = 2;

        [SerializeField]
        private float SemiMinor = 1;

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

        private Vector3[] m_CurrentPath;

        [SerializeField, HideInInspector]
        private EllipseObject m_SavedEllipse;

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

            m_SavedEllipse = new EllipseObject(SemiMajor, SemiMinor, Resolution, GetWorldPath(SemiMajor, SemiMinor, Resolution));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (!m_SavedEllipse.Equals(default(EllipseObject)))
            {
                float radius = MathUtility.GetEllipseCircumference(m_SavedEllipse.SemiMajor, m_SavedEllipse.SemiMinor) / m_SavedEllipse.Resolution * 0.1f;
                for (int i = 0; i < m_SavedEllipse.Path.Length; i++)
                {
                    Gizmos.DrawWireSphere(m_SavedEllipse.Path[i], radius);
                }
            }

            Gizmos.color = Color.red;

            if (!IsValid(SemiMajor, SemiMinor, Resolution))
                return;
            
            float pathSphereRadius = MathUtility.GetEllipseCircumference(SemiMajor, SemiMinor) / Resolution * 0.1f;
            m_CurrentPath = GetWorldPath(SemiMajor, SemiMinor, Resolution);
            for (int i = 0; i < m_CurrentPath.Length; i++)
            {
                Gizmos.DrawWireSphere(m_CurrentPath[i], pathSphereRadius);
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
            Vector3[] positions = new Vector3[resolution + 1];
            for (int i = 0; i <= resolution; i++)
            {
                float angle = i / (float)resolution * 2f * Mathf.PI;
                positions[i] = new Vector3(semiMajor * Mathf.Cos(angle), semiMinor * Mathf.Sin(angle), 0f);
                positions[i] = transform.rotation * positions[i] + transform.position;
            }
            return positions;
        }

        private Vector3[] GetLocalPath(float semiMajor, float semiMinor, int resolution)
        {
            Vector3[] positions = new Vector3[resolution + 1];
            for (int i = 0; i <= resolution; i++)
            {
                float angle = i / (float)resolution * 2f * Mathf.PI;
                positions[i] = new Vector3(semiMajor * Mathf.Cos(angle), semiMinor * Mathf.Sin(angle), 0f);
                positions[i] = transform.rotation * positions[i];
            }
            return positions;
        }
    }
}


