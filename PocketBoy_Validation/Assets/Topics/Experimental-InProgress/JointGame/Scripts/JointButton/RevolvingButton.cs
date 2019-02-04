using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class RevolvingButton : MonoBehaviour
    {
        [SerializeField]
        private RectTransform MovingLink;

        [SerializeField]
        private float Speed = 50f;

        private void Update()
        {
            MovingLink.Rotate(Vector3.up * Speed * Time.deltaTime);
        }
    }
}
