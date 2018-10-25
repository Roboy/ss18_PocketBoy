using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    public class ParabolaTrigger : MonoBehaviour
    {
        [SerializeField]
        private Transform ParabolaEnd;

        public Vector3 ParabolaGoal { get { return ParabolaEnd.position; } }

        private Vector3 m_ParabolaStart;
        
    }
}
