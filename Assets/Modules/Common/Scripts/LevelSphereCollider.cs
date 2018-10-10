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
        public float LoadDuration { get { return SceneLoadDuration; } }

        [SerializeField]
        private float SceneLoadDuration = 2f;
    }
}