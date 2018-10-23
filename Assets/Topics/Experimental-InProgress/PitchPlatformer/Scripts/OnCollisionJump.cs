using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    public class OnCollisionJump : MonoBehaviour
    {
        [SerializeField]
        private float JumpForce = 20f;

        public void OnTriggerEnter(Collider other)
        {
            other.attachedRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            
        }
    }
}

