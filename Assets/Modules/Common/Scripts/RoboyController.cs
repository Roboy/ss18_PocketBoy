using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    /// <summary>
    /// TODO:: Add functionality e.g. startAnimationXY, SayXY
    /// </summary>
    public class RoboyController : MonoBehaviour
    {
        [SerializeField]
        private Transform Head;

        void Start()
        {
            var cameraPosition = Camera.main.transform.position;
            cameraPosition.y = transform.position.y;
            transform.LookAt(cameraPosition);
        }

        void Update()
        {
            Head.LookAt(Camera.main.transform.position);
        }
    }
}


