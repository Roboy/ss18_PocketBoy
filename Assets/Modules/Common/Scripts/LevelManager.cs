namespace Pocketboy.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using GoogleARCore.Examples.HelloAR;
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    /// <summary>
    /// Sets up the level by scanning for planes and placing the Roboy model at a suitable position.
    /// </summary>
    public class LevelManager : Singleton<LevelManager>
    {
        /// <summary>
        /// Roboy prefab.
        /// </summary>
        [SerializeField]
        private RoboyManager RoboyPrefab;

        /// <summary>
        /// Planetsystems include the LevelSpheres which represent a topic to learn about in form of a scene.
        /// </summary>
        [SerializeField]
        private PlanetSystem[] PlanetSystems;

        /// <summary>
        /// Reference to an instantiated copy of the Roboy prefab in a scene.
        /// </summary>
        private RoboyManager m_Roboy;

        private bool m_PlanetSystemsSpawned = false;

        /// <summary>
        /// Cached objects under the same anchor as Roboy for the duration of the current scene.
        /// </summary>
        private List<GameObject> m_RegisteredGameObjects = new List<GameObject>();

        /// <summary>
        /// Objects in the scene that need to be destroyed when exiting a scene, that are not attached to Roboy.
        /// </summary>
        private List<GameObject> m_ObjectsToDestroyOnUnload = new List<GameObject>();

        private void Awake()
        {
            SceneManager.sceneUnloaded += DeleteRegisteredObjects;
            SceneManager.sceneUnloaded += DeleteOtherObjects;
            SceneManager.sceneLoaded += ResetLevel;
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            if (SceneManager.GetActiveScene().name != "HomeScene") // TO THIS SOME OTHER WAY
                return;

            if (m_Roboy == null)
            {
                SpawnRoboy();
            }
            else if (!m_PlanetSystemsSpawned)
            {
                SpawnPlanetSystems();
            }
        }

        /// <summary>
        /// Add objects that should be destroyed on exiting the scene.
        /// </summary>
        /// <param name="gameObj"></param>
        public void RegisterObjectWithLevel(GameObject gameObj)
        {
            m_ObjectsToDestroyOnUnload.Add(gameObj);
        }

        /// <summary>
        /// Parents the given object under the same anchor as Roboy. Use this function when the relative position to roboy should not change during a scene.
        /// </summary>
        public void RegisterGameObjectWithRoboy(GameObject gameObj)
        {
            gameObj.transform.SetParent(m_Roboy.ARAnchor.transform);
            m_RegisteredGameObjects.Add(gameObj);
        }

        public void RegisterGameObjectWithRoboy(GameObject gameObj, Vector3 relativePosition)
        {
            RegisterGameObjectWithRoboy(gameObj);
            PositionGameObjectRelativeToRoboy(gameObj, relativePosition);
        }

        public void RegisterGameObjectWithRoboy(GameObject gameObj, Vector3 relativePosition, Quaternion relativeRotation)
        {
            RegisterGameObjectWithRoboy(gameObj);
            PositionGameObjectRelativeToRoboy(gameObj, relativePosition, relativeRotation);
        }

        public void PositionGameObjectRelativeToRoboy(GameObject gameObj, Vector3 relativePosition, bool createAnchor = false)
        {
            if (createAnchor)
            {
                gameObj.transform.parent = ARSessionManager.Instance.FloorPlane.CreateAnchor(ARSessionManager.Instance.FloorPlane.CenterPose).transform;
            }               
            gameObj.transform.position = m_Roboy.transform.TransformPoint(relativePosition);
        }

        public void PositionGameObjectRelativeToRoboy(GameObject gameObj, Vector3 relativePosition, Quaternion relativeRotation, bool createAnchor = false)
        {
            PositionGameObjectRelativeToRoboy(gameObj, relativePosition, createAnchor);
            gameObj.transform.rotation = m_Roboy.transform.rotation * relativeRotation;
        }

        private void SpawnRoboy()
        {
            if (RoboyManager.InstanceExists)
                return;

            var plane = ARSessionManager.Instance.FloorPlane;
            var anchor = plane.CreateAnchor(plane.CenterPose);
            m_Roboy = Instantiate(RoboyPrefab, plane.CenterPose.position, plane.CenterPose.rotation);
            m_Roboy.transform.parent = anchor.transform;
            m_Roboy.Initialize(anchor);
            SpawnPlanetSystems();
        }

        private void SpawnPlanetSystems()
        {
            if (!RoboyManager.InstanceExists || m_PlanetSystemsSpawned)
                return;

            m_PlanetSystemsSpawned = true;
            var planetSystemOffset = Vector3.zero;
            var planetSystemInitPosition = Vector3.up * 0.5f + Vector3.forward * 0.5f;
            for (int i = 0; i < PlanetSystems.Length; i++)
            {
                var planetSystem = Instantiate(PlanetSystems[i]);       
                int offsetMultiplicator = Mathf.CeilToInt(i / 2f);
                if (i % 2 == 0)
                {
                    offsetMultiplicator *= -1;                    
                }
                planetSystemOffset = offsetMultiplicator * Vector3.right * 0.5f;
                PositionGameObjectRelativeToRoboy(planetSystem.gameObject, planetSystemInitPosition + planetSystemOffset, true);
            }
        }  

        private void ResetLevel(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "HomeScene")
                return;

            m_PlanetSystemsSpawned = false;
        }


        private void DeleteRegisteredObjects(Scene scene)
        {
            for (int i = 0; i < m_RegisteredGameObjects.Count; i++)
            {
                Destroy(m_RegisteredGameObjects[i]);
            }
            m_RegisteredGameObjects.Clear();
        }

        private void DeleteOtherObjects(Scene scene)
        {
            for (int i = 0; i < m_ObjectsToDestroyOnUnload.Count; i++)
            {
                Destroy(m_ObjectsToDestroyOnUnload[i]);
            }
            m_ObjectsToDestroyOnUnload.Clear();
        }
    }
}