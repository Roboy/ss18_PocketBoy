namespace Pocketboy.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using GoogleARCore.Examples.HelloAR;
    using UnityEngine;
    using UnityEngine.UI;

    public class PositionIndicator : MonoBehaviour
    {
        public Camera cam;
        public GameObject indicator;

        private GameObject m_roboyModel;
        private bool m_indicating;
        private bool m_roboyFound = false;
        private Renderer[] m_renderers;

        private void Update()
        {

            //Get a reference to the roboy model
            if (m_roboyModel == null && m_roboyFound == false)
            {
                Debug.Log("No Roboy yet.");
                m_roboyModel = GameObject.FindGameObjectWithTag("Roboy");
                if (m_roboyModel != null)
                {
                    m_roboyFound = true;
                    Debug.Log("There he is!");
                    
                }
            }

            //Get a reference to all of the renderers that roboy has
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
                        indicator.gameObject.SetActive(false);
                        break;
                    }

                    m_indicating = true;
                }
            }

            //In case that the roboy model is not visible, show the position via arrows
            if (m_roboyFound && m_indicating)
            {
                ////Direction in which the camera is looking
                //Vector2 cameraForward = new Vector2(cam.transform.forward.x, cam.transform.forward.z);
                //cameraForward.Normalize();
                ////Direction from camera to roboy
                //Vector2 towardsRoboy = new Vector2(m_roboyModel.transform.position.x - cam.transform.position.x, m_roboyModel.transform.position.z - cam.transform.position.z);
                //towardsRoboy.Normalize();

                //float result = cameraForward.x - towardsRoboy.x;
                //if (result > 0)
                //{
                //    arrowLeft.gameObject.SetActive(true);
                //    arrowRight.gameObject.SetActive(false);
                //}
                //if (result < 0)
                //{
                //    arrowLeft.gameObject.SetActive(false);
                //    arrowRight.gameObject.SetActive(true);
                //}

                indicator.gameObject.SetActive(true);
                Vector3 tmp = new Vector3(m_roboyModel.transform.position.x, m_roboyModel.transform.position.y + 0.5f, m_roboyModel.transform.position.z);
                indicator.transform.LookAt(tmp);
            }


        }
    }
}
