using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
    }
}


