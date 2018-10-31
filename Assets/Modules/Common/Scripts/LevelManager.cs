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
        /// Topics to learn about, represented as spheres/ portals to get into the respective training world.
        /// </summary>
        [SerializeField]
        private List<GameObject> Spheres;

        /// <summary>
        /// Reference to an instantiated copy of the Roboy prefab in a scene.
        /// </summary>
        private RoboyManager m_Roboy;
        /// <summary>
        /// True if one model of Roboy has been spawned.
        /// </summary>
        private bool m_RoboySpawned = false;

        private bool m_LevelSpheresSpawned = false;

        /// <summary>
        /// Reference to the available levels, represented as spheres.
        /// </summary>
        private List<GameObject> m_Levels = new List<GameObject>();

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
            if (SceneManager.GetActiveScene().name != "HomeScene_DEV") // TO THIS SOME OTHER WAY
                return;

            if (!m_RoboySpawned)
            {
                SpawnRoboy();
            }
            else if (!m_LevelSpheresSpawned)
            {
                SpawnLevelSpheres();
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
            gameObj.transform.position = m_Roboy.transform.TransformPoint(relativePosition);
        }

        public void RegisterGameObjectWithRoboy(GameObject gameObj, Vector3 relativePosition, Quaternion relativeRotation)
        {
            RegisterGameObjectWithRoboy(gameObj);
            gameObj.transform.position = m_Roboy.transform.TransformPoint(relativePosition);
            gameObj.transform.rotation = m_Roboy.transform.rotation * relativeRotation;
        }

        private void SpawnRoboy()
        {
            if (m_RoboySpawned)
                return;

            m_RoboySpawned = true;
            var plane = ARSessionManager.Instance.FloorPlane;
            var anchor = plane.CreateAnchor(plane.CenterPose);
            m_Roboy = Instantiate(RoboyPrefab, plane.CenterPose.position, plane.CenterPose.rotation);
            m_Roboy.transform.parent = anchor.transform;
            m_Roboy.Initialize(anchor);
            SpawnLevelSpheres();
        }

        private void SpawnLevelSpheres()
        {
            if (m_Roboy == null || m_LevelSpheresSpawned)
                return;

            m_LevelSpheresSpawned = true;
            var levelSphereInitPosition = new Vector3(m_Roboy.transform.position.x, m_Roboy.transform.position.y + 0.5f, m_Roboy.transform.position.z);
            var levelSphereOffset = Vector3.zero;
            for (int i = 0; i < Spheres.Count; i++)
            {
                var levelSphere = Instantiate(Spheres[i]);
                levelSphere.name = "Level" + (i);

                levelSphereOffset = ((float)(i + 1) / Spheres.Count) * m_Roboy.transform.forward; // WHY FORWARD???? Right does spawns the spheres in front of roboy, dafuq
                if (i % 2 == 1)
                {
                    levelSphereOffset *= -1;
                }

                levelSphere.transform.position = levelSphereInitPosition + levelSphereOffset;
                levelSphere.transform.RotateAround(m_Roboy.transform.position, Vector3.up, 90.0f);
                levelSphere.transform.localScale = levelSphere.transform.localScale * 0.25f;

                var plane = ARSessionManager.Instance.FloorPlane;
                var anchor = plane.CreateAnchor(plane.CenterPose);

                levelSphere.transform.parent = anchor.transform;
                m_Levels.Add(levelSphere);
            }
        }

        private void ResetLevel(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "HomeScene_DEV")
                return;

            m_LevelSpheresSpawned = false;
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
