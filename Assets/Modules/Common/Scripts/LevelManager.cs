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
        public RoboyController RoboyPrefab;

        /// <summary>
        /// Topics to learn about, represented as spheres/ portals to get into the respective training world.
        /// </summary>
        public List<GameObject> Spheres;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        public RoboyController Roboy;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

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

        //private void OnEnable()
        //{
        //    SceneManager.sceneLoaded += ResetLevel;
        //}

        //private void OnDisable()
        //{
        //    SceneManager.sceneLoaded -= ResetLevel;
        //}

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);
            bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            SearchingForPlaneUI.SetActive(showSearchingUI);

            if (!m_RoboySpawned)
            {
                //Call function to spawn Roboy
                SpawnRoboy();
            }
            else if (!m_LevelSpheresSpawned)
            {
                SpawnLevelSpheres();
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        private void SpawnRoboy()
        {
            if (m_AllPlanes.Count == 0 || m_RoboySpawned)
                return;

            m_RoboySpawned = true;
            DetectedPlane plane = m_AllPlanes[0];
            var anchor = plane.CreateAnchor(plane.CenterPose);
            Roboy = Instantiate(RoboyPrefab, plane.CenterPose.position, plane.CenterPose.rotation);
            Roboy.transform.parent = anchor.transform;
            Roboy.Initialize(anchor);
            Debug.Log("Roboy axis:  forward =" + Roboy.transform.forward + "; right ="+Roboy.transform.right + " and up=" + Roboy.transform.up);
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

                DetectedPlane plane = m_AllPlanes[0];
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

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }

}
