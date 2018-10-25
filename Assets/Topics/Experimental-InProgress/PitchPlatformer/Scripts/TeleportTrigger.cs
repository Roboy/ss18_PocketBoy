using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour {

    [SerializeField]
    private Transform TeleportEnd;

    public Vector3 TeleportGoal { get { return TeleportEnd.position; } }
}
