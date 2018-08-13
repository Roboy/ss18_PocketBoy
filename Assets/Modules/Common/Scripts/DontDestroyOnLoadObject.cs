using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class DontDestroyOnLoadObject : MonoBehaviour
    {

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
