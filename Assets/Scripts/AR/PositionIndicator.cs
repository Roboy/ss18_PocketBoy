namespace Pocketboy.AugmentedReality
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using GoogleARCore.Examples.HelloAR;
    using UnityEngine;
    using UnityEngine.UI;

    public class PositionIndicator : MonoBehaviour
    {
        public Text t;
        public Image arrowLeft;
        public Image arrowRight;
        public Camera cam;

        private GameObject m_roboyModel;
        private bool m_indicating;
        private bool m_roboyFound = false;
        private Renderer[] m_renderers;

        private void Update()
        {


            if (m_roboyModel == null && m_roboyFound == false)
            {
                t.text = ("no roboy yet");
                m_roboyModel = GameObject.FindGameObjectWithTag("Roboy");
                if (m_roboyModel != null)
                {
                    m_roboyFound = true;
                    Debug.Log("Roboy found.");
                }
            }

            if (m_renderers == null && m_roboyFound == true)
            {
                m_renderers = m_roboyModel.GetComponentsInChildren<Renderer>();
            }
            else if (m_renderers != null)
            {
                foreach (Renderer r in m_renderers)
                {
                    //As long as one part is still visible, no indicator
                    if (r.isVisible)
                    {
                        m_indicating = false;
                        arrowLeft.gameObject.SetActive(false);
                        arrowRight.gameObject.SetActive(false);
                        break;
                    }

                    m_indicating = true;
                }
            }

            if (m_roboyFound && m_indicating)
            {
                //Direction in which the camera is looking
                Vector2 cameraForward = new Vector2(cam.transform.forward.x, cam.transform.forward.z);
                cameraForward.Normalize();
                //Direction from camera to roboy
                Vector2 towardsRoboy = new Vector2(m_roboyModel.transform.position.x - cam.transform.position.x, m_roboyModel.transform.position.z - cam.transform.position.z);
                towardsRoboy.Normalize();

                float result = cameraForward.x - towardsRoboy.x;
                if (result > 0)
                {
                    arrowLeft.gameObject.SetActive(true);
                    arrowRight.gameObject.SetActive(false);
                }
                if (result < 0)
                {
                    arrowLeft.gameObject.SetActive(false);
                    arrowRight.gameObject.SetActive(true);
                }
            }

            

        }
    }
}
