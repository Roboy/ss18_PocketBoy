using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocketboy.Common
{
    /// <summary>
    /// Script on camera to trigger scene load animation and to cancel it.
    /// </summary>
    public class LevelSphereCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Levelsphere")
            {
                var levelSphere = other.GetComponent<LevelSphere>();
                if (levelSphere != null && !string.IsNullOrEmpty(levelSphere.SceneToLoad))
                {
                    SceneLoadAnimator.Instance.StartAnimation(levelSphere.SceneToLoad);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Levelsphere")
            SceneLoadAnimator.Instance.StopAnimation();
        }
    }
}