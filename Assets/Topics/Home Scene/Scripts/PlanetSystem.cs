using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class PlanetSystem : MonoBehaviour
    {
        [SerializeField]
        private GameObject PathsParent;

        [SerializeField]
        private Sun SunPrefab;

        [SerializeField]
        private TrailRenderer TrailPrefab;

        [SerializeField]
        private LevelSphere[] LevelSpherePrefabs;

        [SerializeField]
        private float LevelSphereSize = 0.025f;

        [SerializeField]
        private float LevelSphereScaleFactor = 2f;

        [SerializeField]
        private float MinCycleDuration = 2f;

        private Sun m_Sun;

        private Collider m_Collider;

        private List<Planet> m_Planets = new List<Planet>();

        private Ellipse[] m_EllipsePaths;

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;

            m_Sun = Instantiate(SunPrefab, transform);

            SetupPlanetPaths();
            SetupLevelSpheres();
            ResumeMoving();
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

        private void OnEnable()
        {
            ResumeMoving();
        }

        private void ResumeMoving()
        {
            for (int i = 0; i < m_Planets.Count; i++)
            {
                m_Planets[i].ZoomOut();
                m_Sun.ZoomOut();
            }
        }

        private void PauseMoving()
        {
            for (int i = 0; i < m_Planets.Count; i++)
            {
                m_Planets[i].ZoomIn();
                m_Sun.ZoomIn();
            }
        }

        private void SetupPlanetPaths()
        {
            //var paths = Instantiate(PathsParent, transform);
            m_EllipsePaths = PathsParent.GetComponentsInChildren<Ellipse>(true);
        }

        private void SetupLevelSpheres()
        {
            for (int i = 0; i < LevelSpherePrefabs.Length && i < m_EllipsePaths.Length; i++)
            {
                var levelSphere = Instantiate(LevelSpherePrefabs[i], transform);
                SetupLevelSphere(levelSphere, i);
            }
        }

        private void SetupLevelSphere(LevelSphere levelSphere, int index)
        {
            levelSphere.transform.localScale = Vector3.one * LevelSphereSize;
            var trail = Instantiate(TrailPrefab, levelSphere.transform);

            Planet planet = null;
            if ((planet = levelSphere.GetComponent<Planet>()) == null)
            {
                planet = levelSphere.gameObject.AddComponent<Planet>();
            }
            planet.Setup(m_EllipsePaths[index], MinCycleDuration * Mathf.Pow(2f, index), LevelSphereScaleFactor); // outer planets are slower than inner planets
            m_Planets.Add(planet);
        }
    }
}
