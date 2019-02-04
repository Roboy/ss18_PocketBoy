using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class DebugCamera : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif
        }
    }
}


