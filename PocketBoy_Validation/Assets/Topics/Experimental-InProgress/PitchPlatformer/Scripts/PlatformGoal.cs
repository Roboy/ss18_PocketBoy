using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    public class PlatformGoal : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem GoalReachedParticle;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlatformPlayer>();
            if (player != null)
            {
                PitchPlatformerEvents.OnReachedGoal();
                Instantiate(GoalReachedParticle, transform);
            }
        }
    }
}


