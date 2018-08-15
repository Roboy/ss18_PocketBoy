using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

namespace Pocketboy.Common
{
    public class DetectedPlaneVisualizer : MonoBehaviour
    {
        public void Initialize(DetectedPlane detectedPlane)
        {
            transform.localScale = new Vector3(detectedPlane.ExtentX, transform.localScale.y, detectedPlane.ExtentZ);
        }
    }
}


