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
        public RoboyManager RoboyPrefab;

        /// <summary>
        /// Topics to learn about, represented as spheres/ portals to get into the respective training world.
        /// </summary>
        public List<GameObject> Spheres;

        public RoboyManager Roboy;

        /// <summary>
        /// True if one model of Roboy has been spawned.
        /// </summary>
        [HideInInspector]
        public bool m_RoboySpawned = false;

        [HideInInspector]
        public bool m_LevelSpheresSpawned = false;

        /// <summary>
        /// Reference to the available levels, represented as spheres.
        /// </summary>
        private List<GameObject> m_Levels = new List<GameObject>();

        private void OnEnable()
        {
            SceneManager.sceneLoaded += ResetLevel;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= ResetLevel;
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            if (!m_RoboySpawned)
            {
                SpawnRoboy();
            }
            else if (!m_LevelSpheresSpawned)
            {
                SpawnLevelSpheres();
            }
        }

        private void SpawnRoboy()
        {
            if (m_RoboySpawned)
                return;

            m_RoboySpawned = true;
            var plane = ARSessionManager.Instance.FloorPlane;
            var anchor = plane.CreateAnchor(plane.CenterPose);
            Roboy = Instantiate(RoboyPrefab, plane.CenterPose.position, plane.CenterPose.rotation);
            Roboy.transform.parent = anchor.transform;
            Roboy.Initialize(anchor);
            SpawnLevelSpheres();
        }

        private void SpawnLevelSpheres()
        {
            if (Roboy == null || m_LevelSpheresSpawned)
                return;

            m_LevelSpheresSpawned = true;
            var levelSphereInitPosition = new Vector3(Roboy.transform.position.x, Roboy.transform.position.y + 0.5f, Roboy.transform.position.z);
            var levelSphereOffset = Vector3.zero;
            for (int i = 0; i < Spheres.Count; i++)
            {
                var levelSphere = Instantiate(Spheres[i]);
                levelSphere.name = "Level" + (i);

                levelSphereOffset = ((float)(i + 1) / Spheres.Count) * Roboy.transform.forward; // WHY FORWARD???? Right does spawns the spheres in front of roboy, dafuq
                if (i % 2 == 1)
                {
                    levelSphereOffset *= -1;
                }

                levelSphere.transform.position = levelSphereInitPosition + levelSphereOffset;
                levelSphere.transform.RotateAround(Roboy.transform.position, Vector3.up, 90.0f);
                levelSphere.transform.localScale = levelSphere.transform.localScale * 0.25f;

                var plane = ARSessionManager.Instance.FloorPlane;
                var anchor = plane.CreateAnchor(plane.CenterPose);

                levelSphere.transform.parent = anchor.transform;
                m_Levels.Add(levelSphere);
            }
        }

        private void ResetLevel(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "HomeScene")
                return;

            m_LevelSpheresSpawned = false;
        }


    }

}
