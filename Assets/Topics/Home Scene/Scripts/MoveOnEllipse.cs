using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class MoveOnEllipse : MonoBehaviour
    {
        [SerializeField]
        private Ellipse EllipseObject;

        [SerializeField]
        private bool OnAwake = false;

        private void Awake()
        {
            if (OnAwake)
                StartMoving();
        }

        public void StartMoving()
        {
            if (EllipseObject == null)
                return;


        }

        public void PauseMoving()
        {

        }

        public void StopMoving()
        {

        }
    }
}