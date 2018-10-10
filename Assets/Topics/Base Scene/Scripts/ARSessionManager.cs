using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using GoogleARCore;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Pocketboy.Common
{

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    public class ARSessionManager : Singleton<ARSessionManager>
    {
        [SerializeField]
        private GameObject CalibrationUI;

        [SerializeField]
        private TextMeshProUGUI InstructionText;

        [SerializeField]
        private Button CalibrateButton;

        [SerializeField]
        private Button ConfirmButton;

        [SerializeField]
        private GameObject FloorObjectPrefab;

        [SerializeField]
        private GameObject DetectedPlaneGenerator;

        [SerializeField]
        private string SceneAfterCalibration;

        public DetectedPlane FloorPlane { get; private set; }

        public float FloorHeight { get; private set; }

        private bool m_CalibrationConfirmed;

        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

        private bool m_IsQuitting = false;

        private void Start()
        {
            CalibrateButton.onClick.AddListener(Calibrate);
            ConfirmButton.onClick.AddListener(Confirm);
        }

        private void Update()
        {
            UpdateApplicationLifecycle();
        }

        private void Calibrate()
        {            
            StartCoroutine(CalibrateFloorInternal());
        }

        private void Confirm()
        {
            m_CalibrationConfirmed = true;
            ConfirmButton.gameObject.SetActive(false);
            CalibrateButton.gameObject.SetActive(true);
        }

        private IEnumerator CalibrateFloorInternal()
        {
            InstructionText.text = "Tap on a plane to place an object.";
            CalibrateButton.gameObject.SetActive(false);

            DetectedPlaneGenerator.SetActive(true);
            
            Transform cameraTransform = Camera.main.transform;
            Anchor lastAnchor = null;

            while (!m_CalibrationConfirmed)
            { 
                // If the player has not touched the screen, we are done with this update.
                if (Input.touchCount < 1 || Input.GetTouch(0).phase != TouchPhase.Began)
                {
                    yield return null;
                    continue;
                }
                Touch touch = Input.GetTouch(0);

                // Raycast against the location the player touched to search for planes.
                TrackableHit hit;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                    TrackableHitFlags.FeaturePointWithSurfaceNormal;

                if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                {
                    // Use hit pose and camera pose to check if hittest is from the
                    // back of the plane, if it is, no need to create the anchor.
                    if (!(hit.Trackable is DetectedPlane))
                        continue;

                    InstructionText.text = "Press the confirm button to select this plane as the floor.";

                    // Only one object can indicate which plane is the floor plane
                    if (lastAnchor != null)
                        Destroy(lastAnchor.gameObject);

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                    // world evolves.
                    lastAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                    
                    var floorObject = Instantiate(FloorObjectPrefab, hit.Pose.position, Quaternion.identity);
                    floorObject.transform.parent = lastAnchor.transform;
                    FloorHeight = hit.Pose.position.y;
                    FloorPlane = hit.Trackable as DetectedPlane;

                    // As soon as an object is created on the floor the user can confirm the plane as the floor plane.
                    ConfirmButton.gameObject.SetActive(true);
                }
                yield return null;
            }

            Destroy(DetectedPlaneGenerator);
            CalibrationUI.SetActive(false);
            if (!string.IsNullOrEmpty(SceneAfterCalibration))
            {
                SceneLoader.Instance.LoadScene(SceneAfterCalibration);
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
                AndroidUtility.ShowMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Application.Quit();
            }
            else if (Session.Status.IsError())
            {
                AndroidUtility.ShowMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Application.Quit();
            }
        }

    }
}


