namespace Pocketboy.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using GoogleARCore.Examples.HelloAR;
    using UnityEngine;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    /// <summary>
    /// Sets up the level by scanning for planes and placing the Roboy model at a suitable position.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {

        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject Roboy;

        /// <summary>
        /// Topics to learn about, represented as spheres/ portals to get into the respective training world.
        /// </summary>
        public List<GameObject> Spheres;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// The rotation in degrees need to apply to model when the Andy model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

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
        private bool m_ModelSpawned = false;

        /// <summary>
        /// Reference to the available levels, represented as spheres.
        /// </summary>
        private List<GameObject> m_Levels = new List<GameObject>();

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

            if (!m_ModelSpawned && m_AllPlanes.Count>0 )
            {
                //Call function to spawn Roboy
                SpawnLevel();
            }

            //if (m_ModelSpawned)
            //    Debug.Log("Distance :" + Vector3.Distance(m_Levels[0].gameObject.transform.position, FirstPersonCamera.transform.position));


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

        private void SpawnLevel()
        {
            //Spawn Roboy
            DetectedPlane tmp = m_AllPlanes[0];
            var anchor = tmp.CreateAnchor(tmp.CenterPose);
            var roboy = Instantiate(Roboy, tmp.CenterPose.position, tmp.CenterPose.rotation);
            roboy.transform.Rotate(90, 0, 0, Space.Self);
            roboy.transform.Rotate(0, 0, k_ModelRotation, Space.Self);
            roboy.transform.parent = anchor.transform;
            m_ModelSpawned = true;
            Debug.Log("roboy spawned.");

            ////Spawn Level spheres
            //for (int i = 1; i < 5; i++)
            //{
            //    GameObject levelSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //    levelSphere.name = "level" + (i);
            //    levelSphere.tag = "Level";
            //    if (i % 2 == 0)
            //    { levelSphere.transform.position = new Vector3(tmp.CenterPose.position.x + ((float)i / 4), tmp.CenterPose.position.y + 0.5f, tmp.CenterPose.position.z); }
            //    if (i % 2 == 1)
            //    { levelSphere.transform.position = new Vector3(tmp.CenterPose.position.x - ((float)i / 4), tmp.CenterPose.position.y + 0.5f, tmp.CenterPose.position.z); }
            //    levelSphere.transform.localScale = levelSphere.transform.localScale * 0.25f;
            //    levelSphere.transform.parent = anchor.transform;
            //    levelSphere.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            //    m_Levels.Add(levelSphere);

            //}

            for (int i = 0; i < Spheres.Count; i++)
            {

                var levelSphere = Instantiate(Spheres[i]);
                levelSphere.name = "Level" + (i);
                levelSphere.tag = "Level";

                //if (i % 2 == 0)
                //{ levelSphere.transform.position = new Vector3(tmp.CenterPose.position.x + ((float)(i+1) / Spheres.Count), tmp.CenterPose.position.y + 0.5f, tmp.CenterPose.position.z); }
                //if (i % 2 == 1)
                //{ levelSphere.transform.position = new Vector3(tmp.CenterPose.position.x - ((float)(i+1) / Spheres.Count), tmp.CenterPose.position.y + 0.5f, tmp.CenterPose.position.z); }
                if (i % 2 == 0)
                { levelSphere.transform.position = new Vector3(roboy.transform.position.x + ((float)(i + 1) / Spheres.Count), roboy.transform.position.y + 0.5f, roboy.transform.position.z); }
                if (i % 2 == 1)
                { levelSphere.transform.position = new Vector3(roboy.transform.position.x - ((float)(i + 1) / Spheres.Count), roboy.transform.position.y + 0.5f, roboy.transform.position.z); }

                levelSphere.transform.RotateAround(roboy.transform.position, Vector3.up, 90.0f);
                levelSphere.transform.localScale = levelSphere.transform.localScale * 0.25f;
                levelSphere.transform.parent = anchor.transform;
                //levelSphere.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                m_Levels.Add(levelSphere);

            }
            Debug.Log("spheres spawned.");
            Debug.Log(m_Levels.Count + " " + m_Levels[0]);
            
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
