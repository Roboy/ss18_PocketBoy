using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

namespace Pocketboy.Common
{
    public class LevelManager2 : Singleton<LevelManager2>
    {
        [SerializeField]
        private RoboyController RoboyPrefab;

        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

        private bool m_IsQuitting = false;

        private bool m_RoboySpawned = false;

        private DetectedPlane m_RoboyPlane;

        private RoboyController m_Roboy;


        void Update()
        {
            UpdateApplicationLifecycle();

            Session.GetTrackables<DetectedPlane>(m_AllPlanes);
            
            if(!m_RoboySpawned)
                CreateLevel();
        }

        private void CreateLevel()
        {
            foreach (var plane in m_AllPlanes)
            {
                if (!(plane.SubsumedBy == null && plane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing))
                    return;

                m_Roboy = Instantiate(RoboyPrefab, plane.CreateAnchor(plane.CenterPose).transform);
                //m_Roboy.transform.localPosition = Vector3.zero;
                //m_Roboy.transform.LookAt(Camera.main.transform.forward);
                m_RoboySpawned = true;
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void UpdateApplicationLifecycle()
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
                Application.Quit();
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Application.Quit();
            }
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


