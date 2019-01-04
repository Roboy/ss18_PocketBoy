using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.Logging
{
    public abstract class SceneLogger : MonoBehaviour
    {
        public abstract string GetStats();

        private void Start()
        {
            LoggerManager.Instance.RegisterLogger(this);
        }
    }
}
