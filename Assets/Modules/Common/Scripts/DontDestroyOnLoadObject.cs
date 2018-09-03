using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class DontDestroyOnLoadObject : MonoBehaviour
    {

        private void Awake()
        {
            DontDestroyOnLoad(transform.root.gameObject);
            
        }

        public void Hack()
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
    }
}
