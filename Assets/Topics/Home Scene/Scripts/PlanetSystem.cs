using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class PlanetSystem : MonoBehaviour
    {
        [SerializeField]
        private LevelSphere[] LevelSpheres;

        [SerializeField]
        private float LevelSphereSize = 0.025f;

        [SerializeField]
        private float LevelSphereScaleFactor = 2f;

        private Collider m_Collider;

        private List<Planet> m_Planets = new List<Planet>();

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;

            SetupLevelSpheres();

            //m_Planets = GetComponentsInChildren<Planet>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlanetSystemCollider"))
            {
                PauseMoving();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlanetSystemCollider"))
            {
                ResumeMoving();
            }
        }

        private void SetupLevelSpheres()
        {
            for (int i = 0; i < LevelSpheres.Length; i++)
            {
                
            }
        }

        private void ResumeMoving()
        {
            for (int i = 0; i < m_Planets.Count; i++)
            {
                m_Planets[i].ZoomOut();
            }
        }

        private void PauseMoving()
        {
            for (int i = 0; i < m_Planets.Count; i++)
            {
                m_Planets[i].ZoomIn();
            }
        }
    }
}
